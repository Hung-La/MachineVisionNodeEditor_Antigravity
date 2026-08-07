using MachineVisionNodeEditor.Builders;
using MachineVisionNodeEditor.Factories;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using MachineVisionNodeEditor.Views.Windows;
using MachineVisionNodeEditor.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MachineVisionNodeEditor.Services
{
    /// <summary>
    /// Dịch vụ lưu/mở file project (.mvne) cho Node Editor.
    /// Sử dụng JSON format với System.Text.Json.
    /// Tái sử dụng pattern snapshot/restore từ ClipboardService.
    /// </summary>
    public class ProjectFileService
    {
        #region DTO Classes (Data Transfer Objects cho serialization)

        /// <summary>
        /// DTO gốc chứa toàn bộ dữ liệu project.
        /// </summary>
        public class ProjectFileDto
        {
            /// <summary>Phiên bản file format để hỗ trợ migration trong tương lai.</summary>
            public int FileVersion { get; set; } = 1;

            /// <summary>Thời điểm lưu file.</summary>
            public string SavedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            /// <summary>Danh sách tất cả node trong diagram.</summary>
            public List<NodeFileDto> Nodes { get; set; } = new();

            /// <summary>Danh sách tất cả connection trong diagram.</summary>
            public List<ConnectionFileDto> Connections { get; set; } = new();

            /// <summary>Trạng thái viewport (zoom, pan).</summary>
            public ViewportStateDto Viewport { get; set; } = new();
        }

        /// <summary>
        /// DTO cho một node.
        /// </summary>
        public class NodeFileDto
        {
            /// <summary>Loại node (enum NodeType).</summary>
            public string NodeType { get; set; } = string.Empty;

            /// <summary>Vị trí X trên canvas.</summary>
            public double X { get; set; }

            /// <summary>Vị trí Y trên canvas.</summary>
            public double Y { get; set; }

            /// <summary>
            /// Các property values của NodePropertyModel.
            /// Chỉ lưu primitive/enum/string - giống pattern của ClipboardService.
            /// </summary>
            public Dictionary<string, JsonElement> Properties { get; set; } = new();
        }

        /// <summary>
        /// DTO cho một connection.
        /// Lưu bằng index (giống ConnectionSnapshot trong ClipboardService).
        /// </summary>
        public class ConnectionFileDto
        {
            /// <summary>Index của node nguồn trong danh sách Nodes.</summary>
            public int FromNodeIndex { get; set; }

            /// <summary>Index của output port trên node nguồn.</summary>
            public int FromPortIndex { get; set; }

            /// <summary>Index của node đích trong danh sách Nodes.</summary>
            public int ToNodeIndex { get; set; }

            /// <summary>Index của input port trên node đích.</summary>
            public int ToPortIndex { get; set; }
        }

        /// <summary>
        /// DTO cho trạng thái viewport.
        /// </summary>
        public class ViewportStateDto
        {
            public double ZoomFactor { get; set; } = 1.0;
            public double OffsetX { get; set; }
            public double OffsetY { get; set; }
        }

        #endregion

        #region Property Snapshot Helpers (tái sử dụng logic từ ClipboardService)

        /// <summary>
        /// Các tên property của NodePropertyModel base class mà KHÔNG nên save.
        /// Đây là metadata hoặc runtime state, không phải algorithm parameters.
        /// </summary>
        private static readonly HashSet<string> _excludedPropertyNames = new()
        {
            nameof(NodePropertyModel.Context),   // Runtime OpenCV state
            nameof(NodePropertyModel.View),      // WPF UI reference
        };

        /// <summary>
        /// Chụp snapshot tất cả property values có thể serialize được từ NodePropertyModel.
        /// Chỉ copy các property có getter+setter, kiểu primitive/enum/string.
        /// </summary>
        private static Dictionary<string, object?> SnapshotPropertyValues(NodePropertyModel propertyModel)
        {
            var values = new Dictionary<string, object?>();
            var type = propertyModel.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (_excludedPropertyNames.Contains(prop.Name))
                    continue;

                if (!IsCopyableType(prop.PropertyType))
                    continue;

                try
                {
                    var value = prop.GetValue(propertyModel);
                    values[prop.Name] = value;
                }
                catch
                {
                    // Bỏ qua nếu không đọc được giá trị
                }
            }

            return values;
        }

        /// <summary>
        /// Áp dụng các property values đã deserialize vào NodePropertyModel.
        /// Xử lý chuyển đổi kiểu từ JsonElement sang kiểu thực tế.
        /// </summary>
        private static void ApplyPropertyValues(NodePropertyModel targetPropertyModel, Dictionary<string, JsonElement> values)
        {
            var type = targetPropertyModel.GetType();

            foreach (var kvp in values)
            {
                var prop = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                    continue;

                try
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var jsonElement = kvp.Value;

                    object? value = ConvertJsonElement(jsonElement, targetType);
                    if (value != null || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    {
                        prop.SetValue(targetPropertyModel, value);
                    }
                }
                catch
                {
                    // Bỏ qua nếu không convert/set được
                }
            }
        }

        /// <summary>
        /// Chuyển đổi JsonElement sang kiểu C# tương ứng.
        /// </summary>
        private static object? ConvertJsonElement(JsonElement element, Type targetType)
        {
            if (element.ValueKind == JsonValueKind.Null)
                return null;

            // String
            if (targetType == typeof(string))
                return element.GetString();

            // Boolean
            if (targetType == typeof(bool))
                return element.GetBoolean();

            // Integer types
            if (targetType == typeof(int))
                return element.GetInt32();
            if (targetType == typeof(byte))
                return (byte)element.GetInt32();
            if (targetType == typeof(short))
                return (short)element.GetInt32();
            if (targetType == typeof(long))
                return element.GetInt64();

            // Floating point
            if (targetType == typeof(double))
                return element.GetDouble();
            if (targetType == typeof(float))
                return (float)element.GetDouble();
            if (targetType == typeof(decimal))
                return element.GetDecimal();

            // Enum - lưu dưới dạng int value
            if (targetType.IsEnum)
            {
                if (element.ValueKind == JsonValueKind.Number)
                    return Enum.ToObject(targetType, element.GetInt32());
                if (element.ValueKind == JsonValueKind.String)
                {
                    var str = element.GetString();
                    if (str != null && Enum.TryParse(targetType, str, out var result))
                        return result;
                }
                return null;
            }

            return null;
        }

        /// <summary>
        /// Kiểm tra xem một kiểu có thể serialize an toàn hay không.
        /// </summary>
        private static bool IsCopyableType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            var checkType = underlyingType ?? type;

            if (checkType.IsPrimitive) return true;
            if (checkType == typeof(string)) return true;
            if (checkType.IsEnum) return true;
            if (checkType == typeof(decimal)) return true;

            return false;
        }

        #endregion

        #region Save

        /// <summary>
        /// Serialize diagram hiện tại thành JSON và lưu vào file.
        /// </summary>
        /// <param name="filePath">Đường dẫn file .mvne</param>
        /// <param name="diagram">ViewModel chứa nodes và connections</param>
        public static void SaveToFile(string filePath, Window_MainWindowViewModel diagram)
        {
            var dto = new ProjectFileDto
            {
                Viewport = new ViewportStateDto
                {
                    ZoomFactor = diagram.ZoomFactor,
                    OffsetX = diagram.OffsetX,
                    OffsetY = diagram.OffsetY
                }
            };

            // Tạo danh sách node DTOs
            var nodeVMs = diagram.Nodes.ToList();
            foreach (var nodeVM in nodeVMs)
            {
                var nodeDto = new NodeFileDto
                {
                    NodeType = nodeVM.NodeModel.Type.ToString(),
                    X = nodeVM.NodeModel.X,
                    Y = nodeVM.NodeModel.Y
                };

                // Snapshot property values
                if (nodeVM.NodePropertyModel != null)
                {
                    var rawValues = SnapshotPropertyValues(nodeVM.NodePropertyModel);
                    // Chuyển sang Dictionary<string, JsonElement> thông qua serialize/deserialize
                    foreach (var kvp in rawValues)
                    {
                        var json = JsonSerializer.Serialize(kvp.Value);
                        nodeDto.Properties[kvp.Key] = JsonDocument.Parse(json).RootElement.Clone();
                    }
                }

                dto.Nodes.Add(nodeDto);
            }

            // Tạo danh sách connection DTOs (dùng index giống ClipboardService)
            foreach (var connVM in diagram.Connections)
            {
                var conn = connVM.ConnectionModel;
                if (conn.FromPort?.Owner == null || conn.ToPort?.Owner == null)
                    continue;

                var fromNode = conn.FromPort.Owner;
                var toNode = conn.ToPort.Owner;

                // Tìm index của node trong danh sách
                int fromNodeIdx = nodeVMs.FindIndex(vm => vm.NodeModel == fromNode);
                int toNodeIdx = nodeVMs.FindIndex(vm => vm.NodeModel == toNode);

                if (fromNodeIdx < 0 || toNodeIdx < 0) continue;

                // Tìm index của port
                int fromPortIdx = -1;
                for (int i = 0; i < fromNode.OutputPorts.Count; i++)
                {
                    if (fromNode.OutputPorts[i].PortModel == conn.FromPort)
                    {
                        fromPortIdx = i;
                        break;
                    }
                }

                int toPortIdx = -1;
                for (int i = 0; i < toNode.InputPorts.Count; i++)
                {
                    if (toNode.InputPorts[i].PortModel == conn.ToPort)
                    {
                        toPortIdx = i;
                        break;
                    }
                }

                if (fromPortIdx >= 0 && toPortIdx >= 0)
                {
                    dto.Connections.Add(new ConnectionFileDto
                    {
                        FromNodeIndex = fromNodeIdx,
                        FromPortIndex = fromPortIdx,
                        ToNodeIndex = toNodeIdx,
                        ToPortIndex = toPortIdx
                    });
                }
            }

            // Serialize ra JSON với formatting đẹp
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var jsonString = JsonSerializer.Serialize(dto, options);
            File.WriteAllText(filePath, jsonString);
        }

        #endregion

        #region Load

        /// <summary>
        /// Đọc file .mvne và khôi phục diagram.
        /// </summary>
        /// <param name="filePath">Đường dẫn file .mvne</param>
        /// <param name="diagram">ViewModel để khôi phục vào</param>
        public static void LoadFromFile(string filePath, Window_MainWindowViewModel diagram)
        {
            var jsonString = File.ReadAllText(filePath);
            var dto = JsonSerializer.Deserialize<ProjectFileDto>(jsonString);

            if (dto == null)
                throw new InvalidDataException("Không thể đọc file project. File có thể bị hỏng.");

            // Clear diagram hiện tại
            diagram.Connections.Clear();
            diagram.Nodes.Clear();
            diagram.SelectionService.Clear();

            // Khôi phục viewport
            diagram.ZoomFactor = dto.Viewport.ZoomFactor;
            diagram.OffsetX = dto.Viewport.OffsetX;
            diagram.OffsetY = dto.Viewport.OffsetY;

            // Tạo lại nodes
            var createdNodes = new List<NodeControl_NodeViewModel>();
            foreach (var nodeDto in dto.Nodes)
            {
                if (!Enum.TryParse<NodeType>(nodeDto.NodeType, out var nodeType))
                    continue;

                // Dùng NodeBuilder để tạo NodeModel (giống AddNode trong MainWindowViewModel)
                var nodeModel = new NodeBuilder()
                    .SetNodeType(nodeType)
                    .SetCoordinate(nodeDto.X, nodeDto.Y)
                    .Build();

                // Dùng NodeFactory để tạo ViewModel
                var nodeVM = NodeFactory.Create(nodeModel);

                // Khôi phục property values
                if (nodeVM.NodePropertyModel != null && nodeDto.Properties.Count > 0)
                {
                    ApplyPropertyValues(nodeVM.NodePropertyModel, nodeDto.Properties);
                }

                diagram.Nodes.Add(nodeVM);
                createdNodes.Add(nodeVM);
            }

            // Tạo lại connections sau khi UI đã render xong
            if (dto.Connections.Count > 0)
            {
                var connections = dto.Connections;
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new Action(() => CreateConnections(diagram, createdNodes, connections)));
            }
        }

        /// <summary>
        /// Tạo lại connections sau khi nodes đã render.
        /// Logic giống PasteNodesCommand.CreateConnections().
        /// </summary>
        private static void CreateConnections(
            Window_MainWindowViewModel diagram,
            List<NodeControl_NodeViewModel> nodeVMs,
            List<ConnectionFileDto> connectionDtos)
        {
            var canvas = MainWindow.Instance.MainCanvas;

            // Khởi tạo Position cho tất cả port
            InitializePortPositions(canvas, nodeVMs);

            foreach (var connDto in connectionDtos)
            {
                if (connDto.FromNodeIndex >= nodeVMs.Count ||
                    connDto.ToNodeIndex >= nodeVMs.Count)
                    continue;

                var fromNodeVM = nodeVMs[connDto.FromNodeIndex];
                var toNodeVM = nodeVMs[connDto.ToNodeIndex];

                var fromNode = fromNodeVM.NodeModel;
                var toNode = toNodeVM.NodeModel;

                // Kiểm tra port index hợp lệ
                if (connDto.FromPortIndex >= fromNode.OutputPorts.Count ||
                    connDto.ToPortIndex >= toNode.InputPorts.Count)
                    continue;

                var fromPort = fromNode.OutputPorts[connDto.FromPortIndex].PortModel;
                var toPort = toNode.InputPorts[connDto.ToPortIndex].PortModel;

                // Kiểm tra kết nối đã tồn tại chưa
                if (diagram.Connections.Any(c =>
                    c.ConnectionModel.FromPort == fromPort &&
                    c.ConnectionModel.ToPort == toPort))
                    continue;

                // Tạo connection
                var connModel = new ConnectionModel();
                connModel.FromPort = fromPort;
                connModel.ToPort = toPort;
                connModel.Start = fromPort.Position;
                connModel.End = toPort.Position;
                connModel.UpdateControls();

                var connVM = new Node_ConnectionViewModel(connModel);
                diagram.Connections.Add(connVM);

                fromPort.IsConnected = true;
                toPort.IsConnected = true;
            }
        }

        /// <summary>
        /// Khởi tạo Position cho tất cả port của các node.
        /// Tái sử dụng logic từ PasteNodesCommand.
        /// </summary>
        private static void InitializePortPositions(Canvas canvas, List<NodeControl_NodeViewModel> nodeVMs)
        {
            var nodesControl = MainWindow.Instance.NodesControl;

            foreach (var nodeVM in nodeVMs)
            {
                var container = nodesControl.ItemContainerGenerator.ContainerFromItem(nodeVM)
                    as FrameworkElement;

                if (container == null) continue;

                var nodeControl = UIHelper.FindVisualChild<NodeControl>(container);
                if (nodeControl == null) continue;

                UpdatePortPositionsFromVisualTree(nodeControl, nodeVM.NodeModel, canvas);
            }
        }

        /// <summary>
        /// Duyệt visual tree để tìm Node_PortView và cập nhật Position.
        /// </summary>
        private static void UpdatePortPositionsFromVisualTree(
            FrameworkElement nodeControl,
            NodeModel nodeModel,
            Canvas canvas)
        {
            var portViews = FindAllVisualChildren<Node_PortView>(nodeControl);

            foreach (var portView in portViews)
            {
                if (portView.DataContext is Node_PortViewModel portVM)
                {
                    try
                    {
                        var position = portView.TransformToAncestor(canvas)
                            .Transform(new Point(portView.ActualWidth / 2, portView.ActualHeight / 2));

                        portVM.PortModel.Position = position;
                    }
                    catch
                    {
                        // Fallback: ước tính từ vị trí node
                        if (portVM.PortModel.Type == PortType.Output)
                            portVM.PortModel.Position = new Point(nodeModel.X + 230, nodeModel.Y + 35);
                        else
                            portVM.PortModel.Position = new Point(nodeModel.X + 5, nodeModel.Y + 35);
                    }
                }
            }
        }

        private static List<T> FindAllVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            var results = new List<T>();
            FindAllVisualChildrenRecursive(parent, results);
            return results;
        }

        private static void FindAllVisualChildrenRecursive<T>(DependencyObject parent, List<T> results) where T : DependencyObject
        {
            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    results.Add(t);
                FindAllVisualChildrenRecursive(child, results);
            }
        }

        #endregion
    }
}

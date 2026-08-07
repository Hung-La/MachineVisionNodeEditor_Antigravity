using MachineVisionNodeEditor.Builders;
using MachineVisionNodeEditor.Factories;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MachineVisionNodeEditor.Services
{
    /// <summary>
    /// Dịch vụ clipboard nội bộ cho node editor.
    /// Lưu trữ bản sao (snapshot) của các node đã chọn và thông tin kết nối giữa chúng.
    /// </summary>
    public class ClipboardService
    {
        /// <summary>
        /// Thông tin snapshot của một node đã copy.
        /// </summary>
        public class NodeSnapshot
        {
            /// <summary>Loại node.</summary>
            public NodeType NodeType { get; set; }

            /// <summary>Vị trí X gốc.</summary>
            public double X { get; set; }

            /// <summary>Vị trí Y gốc.</summary>
            public double Y { get; set; }

            /// <summary>
            /// Lưu trữ giá trị các property của NodePropertyModel.
            /// Key là tên property, Value là giá trị đã snapshot.
            /// Chỉ lưu các property có thể serialize (primitive, enum, string).
            /// </summary>
            public Dictionary<string, object?> PropertyValues { get; set; } = new();
        }

        /// <summary>
        /// Thông tin kết nối giữa các node đã copy.
        /// Lưu bằng index để rebuild sau khi paste.
        /// </summary>
        public class ConnectionSnapshot
        {
            /// <summary>Index của node nguồn trong danh sách snapshot.</summary>
            public int FromNodeIndex { get; set; }

            /// <summary>Index của output port trên node nguồn.</summary>
            public int FromPortIndex { get; set; }

            /// <summary>Index của node đích trong danh sách snapshot.</summary>
            public int ToNodeIndex { get; set; }

            /// <summary>Index của input port trên node đích.</summary>
            public int ToPortIndex { get; set; }
        }

        /// <summary>Danh sách node đã copy.</summary>
        public List<NodeSnapshot> CopiedNodes { get; private set; } = new();

        /// <summary>Danh sách kết nối nội bộ giữa các node đã copy.</summary>
        public List<ConnectionSnapshot> CopiedConnections { get; private set; } = new();

        /// <summary>Có dữ liệu trong clipboard hay không.</summary>
        public bool HasData => CopiedNodes.Count > 0;

        /// <summary>Số lần đã paste (dùng để offset vị trí).</summary>
        public int PasteCount { get; set; }

        /// <summary>
        /// Các tên property của NodePropertyModel base class mà KHÔNG nên copy.
        /// Đây là metadata hoặc runtime state, không phải algorithm parameters.
        /// </summary>
        private static readonly HashSet<string> _excludedPropertyNames = new()
        {
            nameof(NodePropertyModel.Context),   // Runtime OpenCV state
            nameof(NodePropertyModel.View),      // WPF UI reference
        };

        /// <summary>
        /// Chụp snapshot các node đã chọn và kết nối nội bộ giữa chúng.
        /// Bao gồm cả property values của từng node.
        /// </summary>
        /// <param name="selectedNodeVMs">Danh sách ViewModel của các node đã chọn.</param>
        /// <param name="allConnections">Tất cả kết nối trong diagram.</param>
        public void CopyNodes(
            IEnumerable<NodeControl_NodeViewModel> selectedNodeVMs,
            IEnumerable<Node_ConnectionViewModel> allConnections)
        {
            CopiedNodes.Clear();
            CopiedConnections.Clear();
            PasteCount = 0;

            var selectedList = selectedNodeVMs.ToList();
            if (selectedList.Count == 0) return;

            // Tạo snapshot cho từng node (bao gồm property values)
            foreach (var vm in selectedList)
            {
                var snapshot = new NodeSnapshot
                {
                    NodeType = vm.NodeModel.Type,
                    X = vm.NodeModel.X,
                    Y = vm.NodeModel.Y
                };

                // Snapshot property values nếu node có NodePropertyModel
                if (vm.NodePropertyModel != null)
                {
                    snapshot.PropertyValues = SnapshotPropertyValues(vm.NodePropertyModel);
                }

                CopiedNodes.Add(snapshot);
            }

            // Tạo set NodeModel để tra cứu nhanh
            var selectedModels = new HashSet<NodeModel>(selectedList.Select(vm => vm.NodeModel));

            // Tìm các kết nối nội bộ (cả 2 đầu đều nằm trong selection)
            foreach (var connVM in allConnections)
            {
                var conn = connVM.ConnectionModel;
                if (conn.FromPort?.Owner == null || conn.ToPort?.Owner == null)
                    continue;

                var fromNode = conn.FromPort.Owner;
                var toNode = conn.ToPort.Owner;

                if (selectedModels.Contains(fromNode) && selectedModels.Contains(toNode))
                {
                    // Tìm index của node trong danh sách snapshot
                    int fromNodeIdx = selectedList.FindIndex(vm => vm.NodeModel == fromNode);
                    int toNodeIdx = selectedList.FindIndex(vm => vm.NodeModel == toNode);

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
                        CopiedConnections.Add(new ConnectionSnapshot
                        {
                            FromNodeIndex = fromNodeIdx,
                            FromPortIndex = fromPortIdx,
                            ToNodeIndex = toNodeIdx,
                            ToPortIndex = toPortIdx
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Tạo các node mới từ clipboard với offset vị trí.
        /// Property values được khôi phục từ snapshot.
        /// </summary>
        /// <param name="offsetX">Offset X cho mỗi lần paste.</param>
        /// <param name="offsetY">Offset Y cho mỗi lần paste.</param>
        /// <returns>Danh sách ViewModel mới đã tạo.</returns>
        public List<NodeControl_NodeViewModel> CreatePastedNodes(double offsetX = 50, double offsetY = 50)
        {
            var result = new List<NodeControl_NodeViewModel>();

            PasteCount++;
            double totalOffsetX = offsetX * PasteCount;
            double totalOffsetY = offsetY * PasteCount;

            foreach (var snapshot in CopiedNodes)
            {
                var nodeModel = new NodeBuilder()
                    .SetNodeType(snapshot.NodeType)
                    .SetCoordinate(snapshot.X + totalOffsetX, snapshot.Y + totalOffsetY)
                    .Build();

                var vm = NodeFactory.Create(nodeModel);

                // Khôi phục property values từ snapshot vào node mới
                if (vm.NodePropertyModel != null && snapshot.PropertyValues.Count > 0)
                {
                    ApplyPropertyValues(vm.NodePropertyModel, snapshot.PropertyValues);
                }

                result.Add(vm);
            }

            return result;
        }

        #region Property Snapshot Helpers

        /// <summary>
        /// Chụp snapshot tất cả property values có thể copy được từ một NodePropertyModel.
        /// Chỉ copy các property có getter+setter, kiểu primitive/enum/string.
        /// Bỏ qua runtime state (Context, View), ObservableCollection, mảng Point[][], v.v.
        /// </summary>
        private Dictionary<string, object?> SnapshotPropertyValues(NodePropertyModel propertyModel)
        {
            var values = new Dictionary<string, object?>();
            var type = propertyModel.GetType();

            // Lấy tất cả public instance properties (bao gồm cả inherited)
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Bỏ qua property không có getter hoặc setter
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                // Bỏ qua các property trong danh sách loại trừ
                if (_excludedPropertyNames.Contains(prop.Name))
                    continue;

                // Chỉ copy các kiểu có thể serialize an toàn
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
        /// Áp dụng các property values đã snapshot vào một NodePropertyModel mới.
        /// </summary>
        private void ApplyPropertyValues(NodePropertyModel targetPropertyModel, Dictionary<string, object?> values)
        {
            var type = targetPropertyModel.GetType();

            foreach (var kvp in values)
            {
                var prop = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                    continue;

                try
                {
                    var value = kvp.Value;

                    // Đảm bảo kiểu tương thích trước khi set
                    if (value != null && !prop.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        // Thử convert cho nullable types
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        if (value.GetType() != targetType)
                        {
                            try
                            {
                                value = Convert.ChangeType(value, targetType);
                            }
                            catch
                            {
                                continue; // Bỏ qua nếu không convert được
                            }
                        }
                    }

                    prop.SetValue(targetPropertyModel, value);
                }
                catch
                {
                    // Bỏ qua nếu không set được giá trị (VD: setter có validation throw exception)
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem một kiểu có thể copy an toàn hay không.
        /// Cho phép: primitive, enum, string, nullable của chúng.
        /// Từ chối: collections, complex objects, Mat, Point[][], View, Context.
        /// </summary>
        private bool IsCopyableType(Type type)
        {
            // Unwrap nullable
            var underlyingType = Nullable.GetUnderlyingType(type);
            var checkType = underlyingType ?? type;

            // Primitive types (int, double, bool, byte, etc.)
            if (checkType.IsPrimitive)
                return true;

            // String
            if (checkType == typeof(string))
                return true;

            // Enum types (ThresholdTypes, MorphShapes, ColorCode, etc.)
            if (checkType.IsEnum)
                return true;

            // Decimal
            if (checkType == typeof(decimal))
                return true;

            // Từ chối tất cả các kiểu khác (collections, Mat, Point[][], complex objects)
            return false;
        }

        #endregion
    }
}

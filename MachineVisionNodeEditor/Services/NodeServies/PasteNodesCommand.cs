using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    /// <summary>
    /// Lệnh Paste có hỗ trợ Undo/Redo.
    /// Thêm các node đã paste và tạo lại kết nối nội bộ giữa chúng.
    /// Kết nối được tạo sau khi node đã render xong để có vị trí port chính xác.
    /// </summary>
    public class PasteNodesCommand : IUndoableCommand
    {
        private readonly Window_MainWindowViewModel _diagram;
        private readonly List<NodeControl_NodeViewModel> _pastedNodes;
        private readonly List<Node_ConnectionViewModel> _pastedConnections = new();
        private readonly List<ClipboardService.ConnectionSnapshot> _connectionSnapshots;

        public PasteNodesCommand(
            Window_MainWindowViewModel diagram,
            List<NodeControl_NodeViewModel> pastedNodes,
            List<ClipboardService.ConnectionSnapshot> connectionSnapshots)
        {
            _diagram = diagram;
            _pastedNodes = pastedNodes;
            _connectionSnapshots = connectionSnapshots;
        }

        public void Execute()
        {
            // Thêm tất cả node vào diagram
            foreach (var node in _pastedNodes)
            {
                if (!_diagram.Nodes.Contains(node))
                    _diagram.Nodes.Add(node);
            }

            // Chọn các node vừa paste
            _diagram.SelectionService.Clear();
            foreach (var node in _pastedNodes)
            {
                _diagram.SelectionService.Select(node.NodeModel, ctrlPressed: true);
            }

            // Tạo kết nối sau khi UI đã render xong
            // Dùng DispatcherPriority.Loaded để đảm bảo tất cả node views đã tạo và layout xong
            if (_connectionSnapshots.Count > 0)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new Action(() => CreateConnections()));
            }
        }

        /// <summary>
        /// Tạo các kết nối giữa các node đã paste.
        /// Phải gọi sau khi UI render xong để port có vị trí canvas chính xác.
        /// </summary>
        private void CreateConnections()
        {
            _pastedConnections.Clear();

            var canvas = MainWindow.Instance.MainCanvas;

            // Đầu tiên, khởi tạo Position cho TẤT CẢ port của các pasted node
            // Đây là bước quan trọng nhất - nếu không có vị trí đúng, 
            // khi kéo node, connection sẽ nhảy vị trí sai
            InitializePortPositions(canvas);

            foreach (var snapshot in _connectionSnapshots)
            {
                if (snapshot.FromNodeIndex >= _pastedNodes.Count ||
                    snapshot.ToNodeIndex >= _pastedNodes.Count)
                    continue;

                var fromNodeVM = _pastedNodes[snapshot.FromNodeIndex];
                var toNodeVM = _pastedNodes[snapshot.ToNodeIndex];

                var fromNode = fromNodeVM.NodeModel;
                var toNode = toNodeVM.NodeModel;

                // Kiểm tra port index hợp lệ
                if (snapshot.FromPortIndex >= fromNode.OutputPorts.Count ||
                    snapshot.ToPortIndex >= toNode.InputPorts.Count)
                    continue;

                var fromPort = fromNode.OutputPorts[snapshot.FromPortIndex].PortModel;
                var toPort = toNode.InputPorts[snapshot.ToPortIndex].PortModel;

                // Kiểm tra kết nối đã tồn tại chưa
                if (_diagram.Connections.Any(c =>
                    c.ConnectionModel.FromPort == fromPort &&
                    c.ConnectionModel.ToPort == toPort))
                    continue;

                // Tạo connection với vị trí port đã được khởi tạo
                var connModel = new ConnectionModel();
                connModel.FromPort = fromPort;
                connModel.ToPort = toPort;
                connModel.Start = fromPort.Position;
                connModel.End = toPort.Position;
                connModel.UpdateControls();

                var connVM = new Node_ConnectionViewModel(connModel);
                _pastedConnections.Add(connVM);

                _diagram.Connections.Add(connVM);

                fromPort.IsConnected = true;
                toPort.IsConnected = true;
            }
        }

        /// <summary>
        /// Khởi tạo Position cho tất cả port của các node đã paste.
        /// Tìm Node_PortView element trong visual tree và tính vị trí tuyệt đối trên canvas.
        /// </summary>
        private void InitializePortPositions(Canvas canvas)
        {
            var nodesControl = MainWindow.Instance.NodesControl;

            foreach (var nodeVM in _pastedNodes)
            {
                // Tìm container của node trong ItemsControl
                var container = nodesControl.ItemContainerGenerator.ContainerFromItem(nodeVM)
                    as FrameworkElement;

                if (container == null) continue;

                // Tìm NodeControl (có thể là ConvertColor_NodeView, ImageImport_NodeView, etc.)
                var nodeControl = UIHelper.FindVisualChild<NodeControl>(container);
                if (nodeControl == null) continue;

                // Tìm tất cả Node_PortView trong nodeControl
                // và cập nhật Position cho các port tương ứng
                UpdatePortPositionsFromVisualTree(nodeControl, nodeVM.NodeModel, canvas);
            }
        }

        /// <summary>
        /// Duyệt visual tree của một node control để tìm tất cả Node_PortView
        /// và cập nhật Position cho port model tương ứng.
        /// </summary>
        private static void UpdatePortPositionsFromVisualTree(
            FrameworkElement nodeControl,
            NodeModel nodeModel,
            Canvas canvas)
        {
            // Tìm tất cả Node_PortView trong nodeControl
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
                        // TransformToAncestor có thể throw nếu element chưa trong visual tree
                        // Fallback: ước tính từ vị trí node
                        if (portVM.PortModel.Type == PortType.Output)
                            portVM.PortModel.Position = new Point(nodeModel.X + 230, nodeModel.Y + 35);
                        else
                            portVM.PortModel.Position = new Point(nodeModel.X + 5, nodeModel.Y + 35);
                    }
                }
            }
        }

        /// <summary>
        /// Tìm tất cả children kiểu T trong visual tree (không chỉ child đầu tiên).
        /// </summary>
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

        public void Undo()
        {
            // Xóa kết nối trước
            foreach (var connVM in _pastedConnections)
            {
                _diagram.Connections.Remove(connVM);

                if (connVM.ConnectionModel.FromPort != null)
                {
                    connVM.ConnectionModel.FromPort.Connections.Remove(connVM.ConnectionModel);
                    connVM.ConnectionModel.FromPort.IsConnected =
                        connVM.ConnectionModel.FromPort.Connections.Count > 0;
                }

                if (connVM.ConnectionModel.ToPort != null)
                {
                    connVM.ConnectionModel.ToPort.Connections.Remove(connVM.ConnectionModel);
                    connVM.ConnectionModel.ToPort.IsConnected =
                        connVM.ConnectionModel.ToPort.Connections.Count > 0;
                }
            }
            _pastedConnections.Clear();

            // Xóa node
            foreach (var node in _pastedNodes)
            {
                _diagram.Nodes.Remove(node);
            }

            _diagram.SelectionService.Clear();
        }
    }
}

using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MachineVisionNodeEditor.Views.Nodes
{
    public abstract class NodeControl : UserControl
    {
        public static PortModel? DraggingPort { get; set; }
        public NodeControl_NodeViewModel? NodeControl_NodeViewModel { get; set; }

        protected bool _dragging;
        protected Point _dragOrigin;
        protected Point _originPoint;
        protected Canvas? _canvas;


        public NodeControl()
        {
            MouseDown += Header_MouseDown;
        }

        protected void NodeControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is INodeViewModel viewModel)
            {
                viewModel.NodeModel.View = this;
            }
        }

        public static void ClearDraggingPort() => DraggingPort = null;

        public virtual FrameworkElement? GetPortElement(PortModel port) => null;

        protected void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (DataContext is INodeViewModel vm)
            //{
            //    vm.NodeModel.IsSelected = true;
            //}
            //MainWindow.Instance.Window_MainWindowViewModel.ClearAllSelections();

            if (DataContext is INodeViewModel vm)
            {
                if (vm.NodeModel is ISelectable selectable)
                {
                    bool ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                    var selectionService = MainWindow.Instance.Window_MainWindowViewModel.SelectionService;

                    // Nếu node chưa được chọn → select (clear nếu không Ctrl)
                    // Nếu đã chọn → giữ nguyên để kéo nhóm
                    if (!selectable.IsSelected)
                        selectionService.Select(selectable, ctrl);

                    //MainWindow.Instance
                    //    .Window_MainWindowViewModel
                    //    .SelectionService
                    //    .Select(selectable, ctrl);

                    if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                    {
                        if (vm is NodeControl_NodeViewModel viewModel)
                        {
                            if (viewModel.ShowImageCommand.CanExecute(viewModel.NodePropertyModel))
                            {
                                viewModel.ShowImageCommand.Execute(viewModel.NodePropertyModel);
                            }

                        }
                    }

                }

            }

            if (e.LeftButton != MouseButtonState.Pressed) return;
            _canvas = UIHelper.FindVisualParent<Canvas>(this);
            _dragging = true;
            _dragOrigin = e.GetPosition(_canvas);
            _originPoint = e.GetPosition(_canvas);
            ((UIElement)sender).CaptureMouse();
            e.Handled = true;
        }

        protected void Header_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !_dragging || _canvas == null) return;

            Point pos = e.GetPosition(_canvas);
            Vector delta = pos - _dragOrigin;
            _dragOrigin = pos;

            var mainVM = MainWindow.Instance.Window_MainWindowViewModel;

            // Lấy tất cả node đang được chọn và di chuyển cùng lúc
            var selectedNodes = mainVM.Nodes
                .Where(n => n.NodeModel.IsSelected)
                .ToList();

            foreach (var node in selectedNodes)
            {
                double oldX = node.NodeModel.X;
                double oldY = node.NodeModel.Y;

                double targetX = oldX + delta.X;
                double targetY = oldY + delta.Y;

                // Kích thước của MainCanvas
                double canvasWidth = _canvas.ActualWidth > 0 ? _canvas.ActualWidth : mainVM.CanvasWidth;
                double canvasHeight = _canvas.ActualHeight > 0 ? _canvas.ActualHeight : mainVM.CanvasHeight;

                // Kích thước của Node
                double nodeWidth = node.NodeModel.View?.ActualWidth > 0 ? node.NodeModel.View.ActualWidth : 150;
                double nodeHeight = node.NodeModel.View?.ActualHeight > 0 ? node.NodeModel.View.ActualHeight : 100;

                double maxX = Math.Max(0, canvasWidth - nodeWidth);
                double maxY = Math.Max(0, canvasHeight - nodeHeight);

                // Giới hạn chỉ nằm trong MainCanvas
                node.NodeModel.X = Math.Clamp(targetX, 0, maxX);
                node.NodeModel.Y = Math.Clamp(targetY, 0, maxY);

                // Delta thực sau Clamp
                Vector actual = new Vector(
                    node.NodeModel.X - oldX,
                    node.NodeModel.Y - oldY);

                foreach (var p in node.NodeModel.InputPorts) p.PortModel.Position += actual;
                foreach (var p in node.NodeModel.OutputPorts) p.PortModel.Position += actual;

                // Cập nhật dây nối liên quan
                foreach (var conn in mainVM.Connections)
                {
                    bool changed = false;
                    foreach (var p in node.NodeModel.OutputPorts)
                        if (conn.ConnectionModel.FromPort == p.PortModel)
                        { conn.ConnectionModel.Start = p.PortModel.Position; changed = true; }
                    foreach (var p in node.NodeModel.InputPorts)
                        if (conn.ConnectionModel.ToPort == p.PortModel)
                        { conn.ConnectionModel.End = p.PortModel.Position; changed = true; }
                    if (changed) conn.ConnectionModel.UpdateControls();
                }
            }

        }

        protected void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                e.Handled = true;
            }
        }


        protected void Header_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;

            // Dùng INodeViewModel thay vì Node_NodeViewModel
            if (DataContext is INodeViewModel vm)
            {
                Point newPos = new Point(vm.NodeModel.X, vm.NodeModel.Y);
                MainWindow.Instance.Window_MainWindowViewModel.MoveNode(_originPoint, newPos); 
            }
            ((UIElement)sender).ReleaseMouseCapture();
        }

        protected void Port_PortMouseUp(object sender, RoutedPropertyChangedEventArgs<PortModel> e)
        {
            ClearDraggingPort();
            MainWindow.Instance.PreviewPath.Data = null;
        }

    }
}

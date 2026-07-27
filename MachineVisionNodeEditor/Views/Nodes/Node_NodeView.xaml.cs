using MachineVisionNodeEditor.Extensions;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MachineVisionNodeEditor.Views.Nodes
{
    /// <summary>
    /// Interaction logic for Node_NodeView.xaml
    /// </summary>
    public partial class Node_NodeView : NodeControl
    {
        //public static PortModel? DraggingPort { get; set; }
        public Node_NodeViewModel Node_NodeViewModel { get; set; }

        private bool _dragging;
        private Point _dragOrigin;
        private Canvas? _canvas;

        //public Node_NodeView()
        //{
        //    InitializeComponent();
        //}
        public Node_NodeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Node_NodeViewModel vm)
            {
                Node_NodeViewModel = vm;
                var inputPort = new Node_PortViewModel();
                inputPort.PortModel.Type = PortType.Input;
                inputPort.PortModel.Owner = Node_NodeViewModel.NodeModel;
                Node_NodeViewModel.NodeModel.InputPorts.Add(inputPort);
                InputPortsControl.DataContext = Node_NodeViewModel.NodeModel.InputPorts[0];

                var outputPort = new Node_PortViewModel();
                outputPort.PortModel.Type = PortType.Input;
                outputPort.PortModel.Owner = Node_NodeViewModel.NodeModel;
                Node_NodeViewModel.NodeModel.OutputPorts.Add(outputPort);
                OutputPortsControl.DataContext = Node_NodeViewModel.NodeModel.OutputPorts[0];
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────
        /// <summary>Trả về FrameworkElement của port để lấy vị trí.</summary>
        public FrameworkElement? GetPortElement(PortModel port)
            => port.Type == PortType.Input ? InputPortsControl : OutputPortsControl;

        //public static void ClearDraggingPort() => DraggingPort = null;

        //private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    if (e.LeftButton != MouseButtonState.Pressed) return;
        //    _canvas = UIHelper.FindVisualParent<Canvas>(this);
        //    _dragging = true;
        //    _dragOrigin = e.GetPosition(_canvas);
        //    ((UIElement)sender).CaptureMouse();
        //    e.Handled = true;
        //}

        //private void Header_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton != MouseButtonState.Pressed) return;
        //    if (!_dragging || _canvas == null) return;
        //    var pos = e.GetPosition(_canvas);
        //    var delta = pos - _dragOrigin;
        //    _dragOrigin = pos;

        //    if (DataContext is Node_NodeViewModel m)
        //    {
        //        m.NodeModel.X += delta.X; m.NodeModel.Y += delta.Y;

        //        m.NodeModel.OutputPort.Position += delta;
        //        m.NodeModel.InputPort.Position += delta;

        //        var outputConnection = UIHelper.FindConnectionElement(m.NodeModel.OutputPort);
        //        if (outputConnection != null)
        //        {
        //            foreach (var item in outputConnection)
        //            {
        //                var outputConnectionViewModel = item.DataContext as Node_ConnectionViewModel;
        //                if (outputConnectionViewModel != null)
        //                {
        //                    outputConnectionViewModel.ConnectionModel.Start = m.NodeModel.OutputPort.Position;
        //                    outputConnectionViewModel.ConnectionModel.UpdateControls();
        //                }
        //            }

        //        }

        //        var inputConnection = UIHelper.FindConnectionElement(m.NodeModel.InputPort);
        //        if (inputConnection != null)
        //        {
        //            foreach (var item in inputConnection)
        //            {
        //                var inputConnectionViewModel = item.DataContext as Node_ConnectionViewModel;
        //                if (inputConnectionViewModel != null)
        //                {
        //                    inputConnectionViewModel.ConnectionModel.End = m.NodeModel.InputPort.Position;
        //                    inputConnectionViewModel.ConnectionModel.UpdateControls();
        //                }
        //            }
        //        }
        //    }
        //}

        //private void Header_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    _dragging = false;
        //    ((UIElement)sender).ReleaseMouseCapture();
        //}

        //private void Port_PortMouseUp(object sender, RoutedPropertyChangedEventArgs<PortModel> e)
        //{
        //    Node_NodeView.ClearDraggingPort();
        //    MainWindow.Instance.PreviewPath.Data = null;
        //}

    }
}

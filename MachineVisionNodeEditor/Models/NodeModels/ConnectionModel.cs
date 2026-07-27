using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MachineVisionNodeEditor.Models.NodeModels
{
    public class ConnectionModel : BaseViewModel, ISelectable
    {
        private Point _start, _end, _control1, _control2;
        private bool _isSelected;

        public Point Start
        {
            get => _start;
            set { _start = value; OnPropertyChanged(nameof(Start)); }
        }

        public Point End
        {
            get => _end;
            set { _end = value; OnPropertyChanged(nameof(End)); }
        }

        public Point Control1
        {
            get => _control1;
            set { _control1 = value; OnPropertyChanged(nameof(Control1)); OnPropertyChanged(nameof(PathData)); }
        }

        public Point Control2
        {
            get => _control2;
            set { _control2 = value; OnPropertyChanged(nameof(Control2)); OnPropertyChanged(nameof(PathData)); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { SetField(ref _isSelected, value); }
        }

        public double Length
        {
            get
            {
                double dx = End.X - Start.X;
                double dy = End.Y - Start.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        private PortModel? _fromPort;
        public PortModel? FromPort
        {
            get => _fromPort;
            set
            {
                if (_fromPort == value)
                    return;

                if (_fromPort != null)
                    _fromPort.Connections.Remove(this);

                _fromPort = value;

                if (_fromPort != null &&
                    !_fromPort.Connections.Contains(this))
                {
                    _fromPort.Connections.Add(this);
                }

                OnPropertyChanged();

            }
        }

        private PortModel? _toPort;
        public PortModel? ToPort
        {
            get => _toPort;
            set
            {
                if (_toPort == value)
                    return;

                if (_toPort != null)
                    _toPort.Connections.Remove(this);

                _toPort = value;

                if (_toPort != null &&
                    !_toPort.Connections.Contains(this))
                {
                    _toPort.Connections.Add(this);
                }

                OnPropertyChanged();
            }
        }

        public FrameworkElement? View { get; set; }

        public void UpdateControls()
        {
            double dx = (End.X - Start.X) * 0.6;
            Control1 = new Point(Start.X + dx, Start.Y);
            Control2 = new Point(End.X - dx, End.Y);

            //if (ToPort != null)
            //{
            //    ToPort.Connected += ToPort_Connected;
            //}

            OnPropertyChanged(nameof(Length));
        }

        private void ToPort_Connected(PortModel port)
        {
            if (port.Type != PortType.Input)
                return;

            var registry = MainWindow.Instance.Window_MainWindowViewModel.NodeRegistry;

            var startNode = registry.GetViewModel(FromPort.Owner) as NodeControl_NodeViewModel;
            var endNode = registry.GetViewModel(port.Owner) as NodeControl_NodeViewModel;

            if (startNode == null || endNode == null)
                return;

            // Ví dụ copy thông tin
            endNode.NodePropertyModel.Width = startNode.NodePropertyModel.Width;
            endNode.NodePropertyModel.Height = startNode.NodePropertyModel.Height;
        }

        public Geometry PathData
        {
            get
            {
                var p1 = Start;
                var p2 = End;
                double dx = Math.Abs(p2.X - p1.X) * 0.6;

                var fig = new PathFigure { StartPoint = p1, IsFilled = false };
                fig.Segments.Add(new BezierSegment(
                    new Point(p1.X + dx, p1.Y),
                    new Point(p2.X - dx, p2.Y),
                    p2, isStroked: true));

                return new PathGeometry(new[] { fig });
            }
        }

    }
}

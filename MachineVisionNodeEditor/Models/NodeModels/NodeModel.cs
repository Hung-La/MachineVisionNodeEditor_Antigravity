using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineVisionNodeEditor.Models.NodeModels
{
    public enum NodeType
    {
        Test,
        ImageImport,
        ConvertColor,
    }
    public abstract class NodeModel : BaseViewModel, ISelectable
    {
        private string _title;
        private double _x, _y;
        private bool _isSelected;

        public string Title
        {
            get => _title;
            set { _title = value; }
        }

        public double X
        {
            get => _x;
            set { SetField(ref _x, Math.Clamp(value, 10, 14800)); }
        }

        public double Y
        {
            get => _y;
            set { SetField(ref _y, Math.Clamp(value, 10, 14800)); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { SetField(ref _isSelected, value); }
        }

        public ObservableCollection<Node_PortViewModel> InputPorts { get; } = new ObservableCollection<Node_PortViewModel>() { };//Type = PortType.Input };
        public ObservableCollection<Node_PortViewModel> OutputPorts { get; } = new ObservableCollection<Node_PortViewModel>() { };//Type = PortType.Output };

        public NodeControl View
        {
            get;
            set;
        }

        public NodeType Type { get; set; }

        public NodeModel(double x, double y)
        {
            X = x;
            Y = y;
            if (InputPorts.Count != 0)
            {
                foreach (Node_PortViewModel port in InputPorts)
                {
                    port.PortModel.Owner = this;
                }
            }

            if (OutputPorts.Count != 0)
            {
                foreach (Node_PortViewModel port in OutputPorts)
                {
                    port.PortModel.Owner = this;
                }
            }
            //InputPort.Owner = this;
            //OutputPort.Owner = this;

            //InputPort.Type = PortType.Input;
            //OutputPort.Type = PortType.Output;
        }

        public NodeModel() { }

        public Node_PortViewModel AddPort(PortType type)
        {
            var port = new Node_PortViewModel();
            port.PortModel.Type = type;
            port.PortModel.Owner = this;
            //port.PortModel.Connected += OnPortConnected;

            if (type == PortType.Input)
            {
                InputPorts.Add(port);
            }
            else if (type == PortType.Output)
            {
                OutputPorts.Add(port);
            }

            return port;
        }

        private void OnPortConnected(PortModel connectedPort)
        {
            // Ensure there is always at least one free (unconnected) port.
            if (connectedPort.Type == PortType.Input)
            {
                bool hasFree = false;
                foreach (var p in InputPorts)
                    if (!p.PortModel.IsConnected) { hasFree = true; break; }
                if (!hasFree) AddPort(PortType.Input);
            }
            else
            {
                bool hasFree = false;
                foreach (var p in OutputPorts)
                    if (!p.PortModel.IsConnected) { hasFree = true; break; }
                if (!hasFree) AddPort(PortType.Output);
            }
        }
    }
}

using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Erode_NodeViewModel : NodeControl_NodeViewModel<Erode_NodeModel, Erode_NodePropertyModel, Erode_NodeOperationModel>
    {
        public ICommand ShowImageCommand { get; private set; }

        public Erode_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Erode_NodeViewModel(Erode_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Erode_NodeViewModel(NodeModel nodeModel) : base(nodeModel is Erode_NodeModel vm ? vm : new Erode_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.Erode
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Erode";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel.OutputImage != null && !NodePropertyModel.OutputImage.IsDisposed && !NodePropertyModel.OutputImage.Empty(),
                () =>
                {
                    var win = new NodeWindow { DataContext = this };
                    win.Show();
                });
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override Erode_NodeOperationModel CreateOperationModel() => new Erode_NodeOperationModel();
    }
}

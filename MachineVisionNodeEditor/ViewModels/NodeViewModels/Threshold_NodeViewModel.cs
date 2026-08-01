using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Threshold_NodeViewModel : NodeControl_NodeViewModel<Threshold_NodeModel, Threshold_NodePropertyModel, Threshold_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public Threshold_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Threshold_NodeViewModel(Threshold_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Threshold_NodeViewModel(NodeModel nodeModel) : base(nodeModel is Threshold_NodeModel vm ? vm : new Threshold_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.Threshold
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Threshold";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override Threshold_NodeOperationModel CreateOperationModel() => new Threshold_NodeOperationModel();
    }
}

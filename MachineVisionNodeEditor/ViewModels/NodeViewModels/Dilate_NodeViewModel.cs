using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Dilate_NodeViewModel : NodeControl_NodeViewModel<Dilate_NodeModel, Dilate_NodePropertyModel, Dilate_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public Dilate_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Dilate_NodeViewModel(Dilate_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Dilate_NodeViewModel(NodeModel nodeModel) : base(nodeModel is Dilate_NodeModel vm ? vm : new Dilate_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.Dilate
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Dilate";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override Dilate_NodeOperationModel CreateOperationModel() => new Dilate_NodeOperationModel();
    }
}

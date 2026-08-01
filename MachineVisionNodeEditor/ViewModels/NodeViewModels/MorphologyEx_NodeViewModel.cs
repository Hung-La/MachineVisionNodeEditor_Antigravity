using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class MorphologyEx_NodeViewModel : NodeControl_NodeViewModel<MorphologyEx_NodeModel, MorphologyEx_NodePropertyModel, MorphologyEx_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public MorphologyEx_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public MorphologyEx_NodeViewModel(MorphologyEx_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public MorphologyEx_NodeViewModel(NodeModel nodeModel) : base(nodeModel is MorphologyEx_NodeModel vm ? vm : new MorphologyEx_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.MorphologyEx
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "MorphologyEx";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override MorphologyEx_NodeOperationModel CreateOperationModel() => new MorphologyEx_NodeOperationModel();
    }
}

using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class HoughCircles_NodeViewModel : NodeControl_NodeViewModel<HoughCircles_NodeModel, HoughCircles_NodePropertyModel, HoughCircles_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public HoughCircles_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public HoughCircles_NodeViewModel(HoughCircles_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public HoughCircles_NodeViewModel(NodeModel nodeModel) : base(nodeModel is HoughCircles_NodeModel vm ? vm : new HoughCircles_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.HoughCircles
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Hough Circles";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            while (NodeModel.OutputPorts.Count < 2)
            {
                NodeModel.AddPort(PortType.Output);
            }
        }

        protected override HoughCircles_NodeOperationModel CreateOperationModel() => new HoughCircles_NodeOperationModel();
    }
}

using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class ImageRotate_NodeViewModel : NodeControl_NodeViewModel<ImageRotate_NodeModel, ImageRotate_NodePropertyModel, ImageRotate_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public ImageRotate_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageRotate_NodeViewModel(ImageRotate_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageRotate_NodeViewModel(NodeModel nodeModel) : base(nodeModel is ImageRotate_NodeModel vm ? vm : new ImageRotate_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.ImageRotate
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Image Rotate";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override ImageRotate_NodeOperationModel CreateOperationModel() => new ImageRotate_NodeOperationModel();
    }
}

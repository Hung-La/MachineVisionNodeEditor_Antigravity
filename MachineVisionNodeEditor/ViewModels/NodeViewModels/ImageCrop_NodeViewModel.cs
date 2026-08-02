using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class ImageCrop_NodeViewModel : NodeControl_NodeViewModel<ImageCrop_NodeModel, ImageCrop_NodePropertyModel, ImageCrop_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public ImageCrop_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageCrop_NodeViewModel(ImageCrop_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageCrop_NodeViewModel(NodeModel nodeModel) : base(nodeModel is ImageCrop_NodeModel vm ? vm : new ImageCrop_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.ImageCrop
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Image Crop";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override ImageCrop_NodeOperationModel CreateOperationModel() => new ImageCrop_NodeOperationModel();
    }
}

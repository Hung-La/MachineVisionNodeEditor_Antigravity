using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class ImageResize_NodeViewModel : NodeControl_NodeViewModel<ImageResize_NodeModel, ImageResize_NodePropertyModel, ImageResize_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public ImageResize_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageResize_NodeViewModel(ImageResize_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ImageResize_NodeViewModel(NodeModel nodeModel) : base(nodeModel is ImageResize_NodeModel vm ? vm : new ImageResize_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.ImageResize
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Image Resize";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override ImageResize_NodeOperationModel CreateOperationModel() => new ImageResize_NodeOperationModel();
    }
}

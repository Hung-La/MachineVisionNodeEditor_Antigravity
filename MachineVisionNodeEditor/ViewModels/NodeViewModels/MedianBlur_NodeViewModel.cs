using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class MedianBlur_NodeViewModel : NodeControl_NodeViewModel<MedianBlur_NodeModel, MedianBlur_NodePropertyModel, MedianBlur_NodeOperationModel>
    {
        public ICommand ShowImageCommand { get; private set; }

        public MedianBlur_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public MedianBlur_NodeViewModel(MedianBlur_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public MedianBlur_NodeViewModel(NodeModel nodeModel) : base(nodeModel is MedianBlur_NodeModel vm ? vm : new MedianBlur_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.MedianBlur
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Median Blur";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.OutputImage != null && !NodePropertyModel.OutputImage.IsDisposed && !NodePropertyModel.OutputImage.Empty(),
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

        protected override MedianBlur_NodeOperationModel CreateOperationModel() => new MedianBlur_NodeOperationModel();
    }
}

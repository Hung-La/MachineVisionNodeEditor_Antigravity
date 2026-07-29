using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class GaussianBlur_NodeViewModel : NodeControl_NodeViewModel<GaussianBlur_NodeModel, GaussianBlur_NodePropertyModel, GaussianBlur_NodeOperationModel>
    {
        public ICommand ShowImageCommand { get; private set; }

        public GaussianBlur_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public GaussianBlur_NodeViewModel(GaussianBlur_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public GaussianBlur_NodeViewModel(NodeModel nodeModel) : base(nodeModel is GaussianBlur_NodeModel vm ? vm : new GaussianBlur_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.GaussianBlur
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Gaussian Blur";
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

        protected override GaussianBlur_NodeOperationModel CreateOperationModel() => new GaussianBlur_NodeOperationModel();
    }
}

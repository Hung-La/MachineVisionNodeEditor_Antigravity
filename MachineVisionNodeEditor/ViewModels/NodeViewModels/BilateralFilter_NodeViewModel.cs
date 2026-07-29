using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class BilateralFilter_NodeViewModel : NodeControl_NodeViewModel<BilateralFilter_NodeModel, BilateralFilter_NodePropertyModel, BilateralFilter_NodeOperationModel>
    {
        public ICommand ShowImageCommand { get; private set; }

        public BilateralFilter_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public BilateralFilter_NodeViewModel(BilateralFilter_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public BilateralFilter_NodeViewModel(NodeModel nodeModel) : base(nodeModel is BilateralFilter_NodeModel vm ? vm : new BilateralFilter_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.BilateralFilter
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Bilateral Filter";
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

        protected override BilateralFilter_NodeOperationModel CreateOperationModel() => new BilateralFilter_NodeOperationModel();
    }
}

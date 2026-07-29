using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Canny_NodeViewModel : NodeControl_NodeViewModel<Canny_NodeModel, Canny_NodePropertyModel, Canny_NodeOperationModel>
    {
        public ICommand ShowImageCommand { get; private set; }

        public Canny_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Canny_NodeViewModel(Canny_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Canny_NodeViewModel(NodeModel nodeModel) : base(nodeModel is Canny_NodeModel vm ? vm : new Canny_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.Canny
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Canny Edge";
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

        protected override Canny_NodeOperationModel CreateOperationModel() => new Canny_NodeOperationModel();
    }
}

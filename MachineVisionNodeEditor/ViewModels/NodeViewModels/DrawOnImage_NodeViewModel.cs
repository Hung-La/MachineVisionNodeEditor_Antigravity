using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class DrawOnImage_NodeViewModel : NodeControl_NodeViewModel<DrawOnImage_NodeModel, DrawOnImage_NodePropertyModel, DrawOnImage_NodeOperationModel>
    {
        public override ICommand ShowImageCommand { get; protected set; }

        public DrawOnImage_NodeViewModel() : base() 
        {
            Initialize();
            EnsureInitialPorts();
        }

        public DrawOnImage_NodeViewModel(DrawOnImage_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public DrawOnImage_NodeViewModel(NodeModel nodeModel) : base(nodeModel is DrawOnImage_NodeModel vm ? vm : new DrawOnImage_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.DrawOnImage
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Draw On Image";
            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
        }

        private void EnsureInitialPorts()
        {
            while (NodeModel.InputPorts.Count < 2)
            {
                NodeModel.AddPort(PortType.Input);
            }
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override DrawOnImage_NodeOperationModel CreateOperationModel() => new DrawOnImage_NodeOperationModel();
    }
}

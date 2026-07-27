using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Node_NodeViewModel : NodeControl_NodeViewModel<Test_NodeModel, NodePropertyModel, Test_NodeOperationModel>
    {
        //private NodeModel _nodeModel;

        //public NodeModel NodeModel { get => _nodeModel; set { _nodeModel = value; OnPropertyChanged(); } }

        public Node_NodeViewModel(Test_NodeModel nodeModel) : base (nodeModel)
        {
            NodeModel = nodeModel;
            Initialize();
            EnsureInitialPorts();

        }

        public Node_NodeViewModel(NodeModel nodeModel) : base(nodeModel is Test_NodeModel node ? node : new Test_NodeModel())
        {
            Initialize();
            EnsureInitialPorts();
        }

        public Node_NodeViewModel() : base ()
        {
            Initialize();
            EnsureInitialPorts();

        }

        private void Initialize()
        {
            NodeModel.Title = "Test Node";
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override Test_NodeOperationModel CreateOperationModel()
        {
            return new Test_NodeOperationModel();
        }
    }
}

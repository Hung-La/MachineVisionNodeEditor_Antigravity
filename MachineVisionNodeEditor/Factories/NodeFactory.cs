using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Factories
{
    public static class NodeFactory
    {
        public static NodeControl_NodeViewModel Create(NodeType type)
        {
            return type switch
            { 
                NodeType.Test => new Node_NodeViewModel() ,
                NodeType.ImageImport => new ImageImport_NodeViewModel() ,
                NodeType.ConvertColor => new ConvertColor_NodeViewModel() ,
                _ => throw new NotImplementedException()
            };
        }

        public static NodeControl_NodeViewModel Create(NodeModel nodeModel)
        {
            return nodeModel.Type switch
            {
                NodeType.Test => new Node_NodeViewModel(nodeModel),
                NodeType.ImageImport => new ImageImport_NodeViewModel(nodeModel),
                NodeType.ConvertColor => new ConvertColor_NodeViewModel(nodeModel),
                _ => throw new NotImplementedException($"NodeType {nodeModel.Type} not handled")
            };
        }

    }
}

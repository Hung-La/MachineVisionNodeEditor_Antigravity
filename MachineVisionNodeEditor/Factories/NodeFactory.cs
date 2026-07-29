using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;

namespace MachineVisionNodeEditor.Factories
{
    public static class NodeFactory
    {
        public static NodeControl_NodeViewModel Create(NodeType type)
        {
            return type switch
            {
                NodeType.Test => new Node_NodeViewModel(),
                NodeType.ImageImport => new ImageImport_NodeViewModel(),
                NodeType.ConvertColor => new ConvertColor_NodeViewModel(),
                NodeType.Threshold => new Threshold_NodeViewModel(),
                NodeType.GaussianBlur => new GaussianBlur_NodeViewModel(),
                NodeType.MedianBlur => new MedianBlur_NodeViewModel(),
                NodeType.BilateralFilter => new BilateralFilter_NodeViewModel(),
                NodeType.Canny => new Canny_NodeViewModel(),
                NodeType.Erode => new Erode_NodeViewModel(),
                NodeType.Dilate => new Dilate_NodeViewModel(),
                NodeType.MorphologyEx => new MorphologyEx_NodeViewModel(),
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
                NodeType.Threshold => new Threshold_NodeViewModel(nodeModel),
                NodeType.GaussianBlur => new GaussianBlur_NodeViewModel(nodeModel),
                NodeType.MedianBlur => new MedianBlur_NodeViewModel(nodeModel),
                NodeType.BilateralFilter => new BilateralFilter_NodeViewModel(nodeModel),
                NodeType.Canny => new Canny_NodeViewModel(nodeModel),
                NodeType.Erode => new Erode_NodeViewModel(nodeModel),
                NodeType.Dilate => new Dilate_NodeViewModel(nodeModel),
                NodeType.MorphologyEx => new MorphologyEx_NodeViewModel(nodeModel),
                _ => throw new NotImplementedException($"NodeType {nodeModel.Type} not handled")
            };
        }
    }
}

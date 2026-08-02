using MachineVisionNodeEditor.Models.NodeModels;
using System;

namespace MachineVisionNodeEditor.Builders
{
    public class NodeBuilder
    {
        private NodeType _type;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private double _x, _y;

        public NodeBuilder SetNodeType(NodeType type)
        {
            _type = type;
            return this;
        }

        public NodeBuilder SetCoordinate(double X, double Y)
        {
            _x = X;
            _y = Y;
            return this;
        }

        public NodeBuilder SetTitle(string title)
        {
            _title = title;
            return this;
        }

        public NodeBuilder SetDescription(string desciption)
        {
            _description = desciption;
            return this;
        }

        public NodeModel Build()
        {
            NodeModel model = _type switch
            {
                NodeType.ImageImport => new ImageImport_NodeModel(),
                NodeType.ConvertColor => new ConvertColor_NodeModel(),
                NodeType.Test => new Test_NodeModel(),
                NodeType.Threshold => new Threshold_NodeModel(),
                NodeType.GaussianBlur => new GaussianBlur_NodeModel(),
                NodeType.MedianBlur => new MedianBlur_NodeModel(),
                NodeType.BilateralFilter => new BilateralFilter_NodeModel(),
                NodeType.Canny => new Canny_NodeModel(),
                NodeType.Erode => new Erode_NodeModel(),
                NodeType.Dilate => new Dilate_NodeModel(),
                NodeType.MorphologyEx => new MorphologyEx_NodeModel(),
                NodeType.ImageRotate => new ImageRotate_NodeModel(),
                NodeType.ImageResize => new ImageResize_NodeModel(),
                NodeType.ImageCrop => new ImageCrop_NodeModel(),
                NodeType.FindContours => new FindContours_NodeModel(),
                NodeType.DrawContours => new DrawContours_NodeModel(),
                _ => throw new NotImplementedException($"NodeType {_type} not handled in NodeBuilder")
            };

            model.Type = _type;
            model.X = _x;
            model.Y = _y;
            if (!string.IsNullOrEmpty(_title)) model.Title = _title;

            return model;
        }
    }
}

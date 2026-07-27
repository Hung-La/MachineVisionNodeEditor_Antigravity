using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public NodeBuilder SetDescription (string desciption)
        {
            _description = desciption;
            return this;
        }

        public NodeModel Build()
        {
            NodeModel model = _type switch
            {
                NodeType.ImageImport => new ImageImport_NodeModel(), // ✅ đúng subtype
                NodeType.ConvertColor => new ConvertColor_NodeModel(),
                NodeType.Test => new Test_NodeModel()
            };

            model.Type = _type;
            model.X = _x;
            model.Y = _y;
            if (!string.IsNullOrEmpty(_title)) model.Title = _title;

            return model;
        }
    }
}

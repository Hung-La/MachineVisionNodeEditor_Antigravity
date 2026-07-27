using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodeModels
{
    public class ConvertColor_NodeModel : NodeModel
    {
        public ConvertColor_NodeModel() 
        {
            Title = "Convert Color";
            Type = NodeType.ConvertColor;
        }

        public ConvertColor_NodeModel(double x, double y) : base (x,y)
        {
            Title = "Convert Color";
            Type = NodeType.ConvertColor;
        }
    }
}

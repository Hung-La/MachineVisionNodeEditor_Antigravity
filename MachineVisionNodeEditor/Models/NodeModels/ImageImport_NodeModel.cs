using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Models.NodeModels
{
    public class ImageImport_NodeModel : NodeModel
    {

        public ImageImport_NodeModel()
        {
            Title = "Image Import";
            Type = NodeType.ImageImport;
        }

        public ImageImport_NodeModel(double x, double y) : base(x, y)
        {
            Title = "Image Import";
            Type = NodeType.ImageImport;
        }

    }
}

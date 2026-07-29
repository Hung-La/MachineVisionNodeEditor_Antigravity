using OpenCvSharp;
using System.Collections.Generic;

namespace MachineVisionNodeEditor.Models.NodeContextModels
{
    public class NodeContext
    {
        public Dictionary<string, object> Inputs { get; } = new();
        public Dictionary<string, object> Outputs { get; } = new();

        public Mat? InputImage
        {
            get => Inputs.TryGetValue("Image", out var v) ? v as Mat : null;
            set
            {
                if (value != null)
                    Inputs["Image"] = value;
                else
                    Inputs.Remove("Image");
            }
        }

        public Mat? OutputImage
        {
            get => Outputs.TryGetValue("Image", out var v) ? v as Mat : null;
            set
            {
                if (value != null)
                    Outputs["Image"] = value;
                else
                    Outputs.Remove("Image");
            }
        }
    }
}

using MachineVisionAlgorithm.Filter;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class Canny_NodeOperationModel : NodeOperationModel<Canny_NodePropertyModel>
    {
        public override void Execute(Canny_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.Context.OutputImage = Filter.ApplyCanny(sourceImage, property.Threshold1, property.Threshold2, property.ApertureSize, property.L2Gradient);
        }
    }
}

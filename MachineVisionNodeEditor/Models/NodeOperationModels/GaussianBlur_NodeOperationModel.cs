using MachineVisionAlgorithm.Filter;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class GaussianBlur_NodeOperationModel : NodeOperationModel<GaussianBlur_NodePropertyModel>
    {
        public override void Execute(GaussianBlur_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.OutputImage = Filter.ApplyGaussianBlur(sourceImage, property.KSize, property.SigmaX);
        }
    }
}

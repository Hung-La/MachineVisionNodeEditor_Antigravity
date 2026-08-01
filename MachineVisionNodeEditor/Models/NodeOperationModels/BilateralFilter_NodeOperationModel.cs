using MachineVisionAlgorithm.Filter;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class BilateralFilter_NodeOperationModel : NodeOperationModel<BilateralFilter_NodePropertyModel>
    {
        public override void Execute(BilateralFilter_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.Context.OutputImage = Filter.ApplyBilateralFilter(sourceImage, property.D, property.SigmaColor, property.SigmaSpace);
        }
    }
}

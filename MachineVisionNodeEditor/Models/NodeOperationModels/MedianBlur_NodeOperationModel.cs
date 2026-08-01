using MachineVisionAlgorithm.Filter;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class MedianBlur_NodeOperationModel : NodeOperationModel<MedianBlur_NodePropertyModel>
    {
        public override void Execute(MedianBlur_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.Context.OutputImage = Filter.ApplyMedianBlur(sourceImage, property.KSize);
        }
    }
}

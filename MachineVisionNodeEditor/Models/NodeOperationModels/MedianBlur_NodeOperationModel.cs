using MachineVisionAlgorithm.Filter;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class MedianBlur_NodeOperationModel : NodeOperationModel<MedianBlur_NodePropertyModel>
    {
        public override void Execute(MedianBlur_NodePropertyModel property)
        {
            var sourceImage = property.Inputs.TryGetValue("Image", out var src) ? src as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.OutputImage = Filter.ApplyMedianBlur(sourceImage, property.KSize);
        }
    }
}

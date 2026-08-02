using MachineVisionAlgorithm.ImageTransform;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class ImageRotate_NodeOperationModel : NodeOperationModel<ImageRotate_NodePropertyModel>
    {
        public override void Execute(ImageRotate_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = ImageTransform.ApplyRotate(
                    sourceImage,
                    property.Angle,
                    property.Scale,
                    property.Interpolation,
                    property.BorderMode);
            }
        }
    }
}

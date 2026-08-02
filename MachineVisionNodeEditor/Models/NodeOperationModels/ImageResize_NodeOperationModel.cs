using MachineVisionAlgorithm.ImageTransform;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class ImageResize_NodeOperationModel : NodeOperationModel<ImageResize_NodePropertyModel>
    {
        public override void Execute(ImageResize_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = ImageTransform.ApplyResize(
                    sourceImage,
                    property.TargetWidth,
                    property.TargetHeight,
                    property.ScaleX,
                    property.ScaleY,
                    property.UseScaleFactor,
                    property.Interpolation);
            }
        }
    }
}

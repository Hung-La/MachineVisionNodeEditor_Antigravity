using MachineVisionAlgorithm.ImageTransform;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class ImageCrop_NodeOperationModel : NodeOperationModel<ImageCrop_NodePropertyModel>
    {
        public override void Execute(ImageCrop_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = ImageTransform.ApplyCrop(
                    sourceImage,
                    property.CropX,
                    property.CropY,
                    property.CropWidth,
                    property.CropHeight);
            }
        }
    }
}

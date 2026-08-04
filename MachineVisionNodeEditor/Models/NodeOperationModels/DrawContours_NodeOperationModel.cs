using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class DrawContours_NodeOperationModel : NodeOperationModel<DrawContours_NodePropertyModel>
    {
        public override void Execute(DrawContours_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            var contours = property.Context.Inputs.TryGetValue("Contours", out var contour) ? contour.Value as Point[][] : property.Context.Get<Point[][]>("Contours");

            property.TotalContours = contours != null ? contours.Length : 0;

            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                if (contours != null && contours.Length > 0)
                {
                    property.Context.OutputImage = DrawContours.ApplyDrawContours(
                        sourceImage, 
                        contours,
                        property.GetScalarColor(), 
                        property.Thickness,
                        property.ShowText);
                }
                else
                {
                    property.Context.OutputImage = sourceImage.Clone();
                }
            }
            else
            {
                property.Context.OutputImage = null;
            }
        }
    }
}

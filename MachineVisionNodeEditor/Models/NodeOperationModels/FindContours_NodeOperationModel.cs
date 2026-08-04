using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class FindContours_NodeOperationModel : NodeOperationModel<FindContours_NodePropertyModel>
    {
        public override void Execute(FindContours_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = FindContours.ApplyFindContours(
                    sourceImage,
                    property.Mode,
                    property.Method,
                    new Scalar(0, 255, 0),
                    property.Thickness,
                    out var contours,
                    out _);

                property.ContourCount = contours != null ? contours.Length : 0;
                property.Context.Set<Point[][]>("Contours", contours);
            }
            else
            {
                property.Context.OutputImage = null;
                property.Context.Set<Point[][]>("Contours", null);
            }
        }
    }
}

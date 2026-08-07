using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class DrawOnImage_NodeOperationModel : NodeOperationModel<DrawOnImage_NodePropertyModel>
    {
        public override void Execute(DrawOnImage_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;

            // Retrieve any geometry input passed to Port 1 or stored in Context
            object? drawData = null;

            if (property.Context.Inputs.TryGetValue("Contours", out var contourInput) && contourInput.Value != null)
            {
                drawData = contourInput.Value;
            }
            else if (property.Context.Inputs.TryGetValue("Lines", out var lineInput) && lineInput.Value != null)
            {
                drawData = lineInput.Value;
            }
            else if (property.Context.Inputs.TryGetValue("Circles", out var circleInput) && circleInput.Value != null)
            {
                drawData = circleInput.Value;
            }
            else if (property.Context.Inputs.TryGetValue("Points", out var pointInput) && pointInput.Value != null)
            {
                drawData = pointInput.Value;
            }
            else if (property.Context.Inputs.TryGetValue("Points2f", out var point2fInput) && point2fInput.Value != null)
            {
                drawData = point2fInput.Value;
            }
            else
            {
                drawData = property.Context.Get<object>("DrawData")
                        ?? property.Context.Get<object>("Contours")
                        ?? property.Context.Get<object>("Lines")
                        ?? property.Context.Get<object>("Circles")
                        ?? property.Context.Get<object>("Points2f")
                        ?? property.Context.Get<object>("Points");
            }

            int count = 0;
            if (drawData is Point[][] contours) count = contours.Length;
            else if (drawData is LineSegmentPoint[] lines) count = lines.Length;
            else if (drawData is CircleSegment[] circles) count = circles.Length;
            else if (drawData is Point2f[] pts2f) count = pts2f.Length;
            else if (drawData is Point[] pts) count = pts.Length;
            else if (drawData is Point2f || drawData is Point) count = 1;

            property.TotalContours = count;

            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                if (drawData != null)
                {
                    property.Context.OutputImage = DrawOnImage.ApplyDrawOnImage(
                        sourceImage, 
                        drawData,
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

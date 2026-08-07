using MachineVisionAlgorithm.HoughTransform;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;
using System.Linq;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class HoughCircles_NodeOperationModel : NodeOperationModel<HoughCircles_NodePropertyModel>
    {
        public override void Execute(HoughCircles_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = HoughCircles.ApplyHoughCircles(
                    sourceImage,
                    property.Dp,
                    property.MinDist,
                    property.Param1,
                    property.Param2,
                    property.MinRadius,
                    property.MaxRadius,
                    property.GetScalarColor(),
                    property.Thickness,
                    out int totalCirclesCount,
                    out CircleSegment[] circles);

                property.TotalCircles = totalCirclesCount;
                property.Circles = circles;

                property.Context.Set<CircleSegment[]>("Circles", circles);

                // Convert circles into Point[][] (ellipse polygon contours) so DrawContours node can also handle drawing them!
                if (circles != null && circles.Length > 0)
                {
                    Point[][] contoursFromCircles = circles.Select(c =>
                        Cv2.Ellipse2Poly(new Point((int)System.Math.Round(c.Center.X), (int)System.Math.Round(c.Center.Y)),
                                         new Size((int)System.Math.Round(c.Radius), (int)System.Math.Round(c.Radius)),
                                         0, 0, 360, 5)
                    ).ToArray();
                    property.Context.Set<Point[][]>("Contours", contoursFromCircles);
                }
                else
                {
                    property.Context.Set<Point[][]>("Contours", null);
                }
            }
            else
            {
                property.Context.OutputImage = null;
                property.Context.Set<CircleSegment[]>("Circles", null);
                property.Context.Set<Point[][]>("Contours", null);
            }
        }
    }
}

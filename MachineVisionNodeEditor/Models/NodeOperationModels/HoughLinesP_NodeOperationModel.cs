using MachineVisionAlgorithm.HoughTransform;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;
using System.Linq;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class HoughLinesP_NodeOperationModel : NodeOperationModel<HoughLinesP_NodePropertyModel>
    {
        public override void Execute(HoughLinesP_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = HoughLinesP.ApplyHoughLinesP(
                    sourceImage,
                    property.Rho,
                    property.Theta,
                    property.Threshold,
                    property.MinLineLength,
                    property.MaxLineGap,
                    property.GetScalarColor(),
                    property.Thickness,
                    out int totalLinesCount,
                    out LineSegmentPoint[] lines);

                property.TotalLines = totalLinesCount;
                property.Lines = lines;

                property.Context.Set<LineSegmentPoint[]>("Lines", lines);

                // Convert lines into Point[][] format so DrawContours or other contour nodes can also handle them
                if (lines != null)
                {
                    Point[][] contoursFromLines = lines.Select(l => new Point[] { l.P1, l.P2 }).ToArray();
                    property.Context.Set<Point[][]>("Contours", contoursFromLines);
                }
                else
                {
                    property.Context.Set<Point[][]>("Contours", null);
                }
            }
            else
            {
                property.Context.OutputImage = null;
                property.Context.Set<LineSegmentPoint[]>("Lines", null);
                property.Context.Set<Point[][]>("Contours", null);
            }
        }
    }
}

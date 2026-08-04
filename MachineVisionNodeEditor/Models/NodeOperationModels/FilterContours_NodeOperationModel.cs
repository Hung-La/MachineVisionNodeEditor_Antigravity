using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class FilterContours_NodeOperationModel : NodeOperationModel<FilterContours_NodePropertyModel>
    {
        public override void Execute(FilterContours_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = FilterContours.ApplyFilterContours(
                    sourceImage,
                    property.AreaMin,
                    property.AreaMax,
                    property.GetScalarColor(),
                    property.Thickness,
                    out int totalCount,
                    out int drawnCount,
                    out Point[][] contours);

                property.TotalContours = totalCount;
                property.DrawnContours = drawnCount;
                property.Contours = contours;

                property.Context.Set<Point[][]>("Contours", contours);
            }
        }
    }
}

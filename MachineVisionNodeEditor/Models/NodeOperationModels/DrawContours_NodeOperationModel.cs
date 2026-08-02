using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class DrawContours_NodeOperationModel : NodeOperationModel<DrawContours_NodePropertyModel>
    {
        public override void Execute(DrawContours_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                property.Context.OutputImage = DrawContours.ApplyDrawContours(
                    sourceImage,
                    property.AreaMin,
                    property.AreaMax,
                    property.GetScalarColor(),
                    property.Thickness,
                    out int totalCount,
                    out int drawnCount);

                property.TotalContours = totalCount;
                property.DrawnContours = drawnCount;
            }
        }
    }
}

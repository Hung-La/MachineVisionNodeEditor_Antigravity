using MachineVisionAlgorithm.Morphology;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class Erode_NodeOperationModel : NodeOperationModel<Erode_NodePropertyModel>
    {
        public override void Execute(Erode_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.Context.OutputImage = Morphology.ApplyErode(sourceImage, property.Shape, property.KSize, property.Iterations);
        }
    }
}

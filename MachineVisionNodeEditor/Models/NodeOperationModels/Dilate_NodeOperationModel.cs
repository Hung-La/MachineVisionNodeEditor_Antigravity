using MachineVisionAlgorithm.Morphology;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class Dilate_NodeOperationModel : NodeOperationModel<Dilate_NodePropertyModel>
    {
        public override void Execute(Dilate_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.Context.OutputImage = Morphology.ApplyDilate(sourceImage, property.Shape, property.KSize, property.Iterations);
        }
    }
}

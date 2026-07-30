using MachineVisionAlgorithm.Morphology;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class MorphologyEx_NodeOperationModel : NodeOperationModel<MorphologyEx_NodePropertyModel>
    {
        public override void Execute(MorphologyEx_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.OutputImage = Morphology.ApplyMorphologyEx(sourceImage, property.Operation, property.Shape, property.KSize, property.Iterations);
        }
    }
}

using MachineVisionAlgorithm.Binary;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class Threshold_NodeOperationModel : NodeOperationModel<Threshold_NodePropertyModel>
    {
        public override void Execute(Threshold_NodePropertyModel property)
        {
            var sourceImage = property.Inputs.TryGetValue("Image", out var src) ? src as Mat : null;
            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
                property.OutputImage = Threshold.ApplyThreshold(sourceImage, property.Thresh, property.MaxVal, property.SelectedType);
        }
    }
}

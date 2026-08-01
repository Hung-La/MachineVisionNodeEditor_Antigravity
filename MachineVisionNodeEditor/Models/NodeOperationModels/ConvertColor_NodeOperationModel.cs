using MachineVisionAlgorithm.ConvertColor;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class ConvertColor_NodeOperationModel : NodeOperationModel<ConvertColor_NodePropertyModel>
    {
        public override void Execute(ConvertColor_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : null;
            if (sourceImage != null)
                property.Context.OutputImage = ConvertColor.Convert(sourceImage, (ColorConversionCodes)property.SelectedCode);
        }
    }
}

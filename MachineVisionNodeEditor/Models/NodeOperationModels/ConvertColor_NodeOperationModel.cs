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
            property.DestinationImage = ConvertColor.Convert(property.SourceImage, (ColorConversionCodes)property.SelectedCode);
        }
    }
}

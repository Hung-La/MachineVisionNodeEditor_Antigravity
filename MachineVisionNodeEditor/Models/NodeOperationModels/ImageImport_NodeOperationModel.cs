using MachineVisionAlgorithm.ImageInteraction;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class ImageImport_NodeOperationModel : NodeOperationModel<ImageImport_NodePropertyModel>
    {

        public override void Execute(ImageImport_NodePropertyModel propertyModel)
        {
            if (string.IsNullOrWhiteSpace(propertyModel.FilePath))
                return;

            propertyModel.DestinationImage = ImageImport.ReadImage(propertyModel.FilePath, (ImreadModes)propertyModel.SelectedMode);

        }
    }
}

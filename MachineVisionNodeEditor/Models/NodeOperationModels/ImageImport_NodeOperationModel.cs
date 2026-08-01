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

            var mode = propertyModel.SelectedMode != null ? (ImreadModes)propertyModel.SelectedMode : ImreadModes.Color;
            var img = ImageImport.ReadImage(propertyModel.FilePath, mode);
            propertyModel.Context.InputImage = img;
            propertyModel.Context.OutputImage = img;
            if (img != null)
            {
                propertyModel.Context.OutputImages = new List<Mat> { img };
            }
        }
    }
}

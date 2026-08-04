using MachineVisionAlgorithm.Contours;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public class DrawContours_NodeOperationModel : NodeOperationModel<DrawContours_NodePropertyModel>
    {
        public override void Execute(DrawContours_NodePropertyModel property)
        {
            var sourceImage = property.Context.Inputs.TryGetValue("Image", out var src) ? src.Value as Mat : property.Context.InputImage;
            var contours = property.Context.Inputs.TryGetValue("Contours", out var contour) ? contour.Value as Point[][] : property.Context.Get<Point[][]>("Contours");

            if (sourceImage != null && !sourceImage.IsDisposed && !sourceImage.Empty())
            {
                if (contours != null && contours.Length > 0)
                {
                    property.Context.OutputImage = DrawContours.ApplyDrawContours(
                        sourceImage, contours,
                        Scalar.Green, 
                        property.Thickness);
                }
                else
                {
                    property.Context.OutputImage = sourceImage.Clone();
                }
            }
            else
            {
                property.Context.OutputImage = null;
            }
        }
    }
}

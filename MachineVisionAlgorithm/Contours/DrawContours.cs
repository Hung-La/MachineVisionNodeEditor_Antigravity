using OpenCvSharp;
using System;

namespace MachineVisionAlgorithm.Contours
{
    [Obsolete("Use DrawOnImage instead.")]
    public static class DrawContours
    {
        public static Mat ApplyDrawContours(
            Mat sourceImage, 
            object? drawData,
            Scalar color,
            int thickness = 2,
            bool showText = true)
        {
            return DrawOnImage.ApplyDrawOnImage(sourceImage, drawData, color, thickness, showText);
        }
    }
}

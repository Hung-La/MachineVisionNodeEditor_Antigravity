using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionAlgorithm.ConvertColor
{
    public class ConvertColor
    {
        public static Mat Convert(Mat sourceImage, ColorConversionCodes colorCode)
        {
            Mat dst = new Mat();

            Cv2.CvtColor(sourceImage, dst, colorCode);

            return dst;
        }
    }
}

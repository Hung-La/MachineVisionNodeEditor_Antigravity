using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionAlgorithm.ImageInteraction
{
    public class ImageImport
    {
        public static Mat ReadImage(string inputPath, ImreadModes mode = ImreadModes.Unchanged)
        {
            return Cv2.ImRead(inputPath, mode);
        }

    }
}

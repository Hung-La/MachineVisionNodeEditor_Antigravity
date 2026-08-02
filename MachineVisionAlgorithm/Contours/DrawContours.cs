using OpenCvSharp;

namespace MachineVisionAlgorithm.Contours
{
    public static class DrawContours
    {
        public static Mat ApplyDrawContours(
            Mat src,
            double areaMin,
            double areaMax,
            Scalar color,
            int thickness,
            out int totalContours,
            out int drawnContours)
        {

            totalContours = 0;
            drawnContours = 0;

            if (src == null || src.IsDisposed || src.Empty()) return new Mat();

            Mat gray = src;
            bool needDisposeGray = false;

            if (src.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                needDisposeGray = true;
            }

            using Mat binaryCopy = gray.Clone();
            Cv2.FindContours(binaryCopy, out Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (needDisposeGray)
            {
                gray.Dispose();
            }

            Mat dst = new Mat();
            if (src.Channels() == 1)
            {
                Cv2.CvtColor(src, dst, ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                dst = src.Clone();
            }

            if (contours != null)
            {
                totalContours = contours.Length;
                double minA = System.Math.Min(areaMin, areaMax);
                double maxA = System.Math.Max(areaMin, areaMax);

                foreach (var contour in contours)
                {
                    double area = Cv2.ContourArea(contour);
                    if (area >= minA && area <= maxA)
                    {
                        Cv2.DrawContours(dst, new Point[][] { contour }, -1, color, thickness);
                        drawnContours++;
                    }
                }
            }

            return dst;

        }
    }
}

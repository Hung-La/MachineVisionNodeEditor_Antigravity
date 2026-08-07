using OpenCvSharp;
using System;

namespace MachineVisionAlgorithm.HoughTransform
{
    public static class HoughCircles
    {
        public static Mat ApplyHoughCircles(
            Mat src,
            double dp,
            double minDist,
            double param1,
            double param2,
            int minRadius,
            int maxRadius,
            Scalar color,
            int thickness,
            out int totalCircles,
            out CircleSegment[] circles)
        {
            totalCircles = 0;
            circles = Array.Empty<CircleSegment>();

            if (src == null || src.IsDisposed || src.Empty()) return new Mat();

            Mat gray = src;
            bool needDisposeGray = false;

            if (src.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                needDisposeGray = true;
            }

            double actualDp = dp <= 0 ? 1.0 : dp;
            double actualMinDist = minDist <= 0 ? 10.0 : minDist;

            circles = Cv2.HoughCircles(gray, HoughModes.Gradient, actualDp, actualMinDist, param1, param2, minRadius, maxRadius);

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

            if (circles != null)
            {
                totalCircles = circles.Length;
                foreach (var circle in circles)
                {
                    Point center = new Point((int)Math.Round(circle.Center.X), (int)Math.Round(circle.Center.Y));
                    int radius = (int)Math.Round(circle.Radius);

                    // Draw circle outline
                    Cv2.Circle(dst, center, radius, color, thickness, LineTypes.AntiAlias);
                    // Draw center point
                    Cv2.Circle(dst, center, 2, color, -1, LineTypes.AntiAlias);
                }
            }

            return dst;
        }
    }
}

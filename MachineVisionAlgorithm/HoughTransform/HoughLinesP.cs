using OpenCvSharp;
using System;

namespace MachineVisionAlgorithm.HoughTransform
{
    public static class HoughLinesP
    {
        public static Mat ApplyHoughLinesP(
            Mat src,
            double rho,
            double thetaDegrees,
            int threshold,
            double minLineLength,
            double maxLineGap,
            Scalar color,
            int thickness,
            out int totalLines,
            out LineSegmentPoint[] lines)
        {
            totalLines = 0;
            lines = Array.Empty<LineSegmentPoint>();

            if (src == null || src.IsDisposed || src.Empty()) return new Mat();

            Mat gray = src;
            bool needDisposeGray = false;

            if (src.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                needDisposeGray = true;
            }

            double thetaRad = (thetaDegrees <= 0 ? 1.0 : thetaDegrees) * Math.PI / 180.0;
            double actualRho = rho <= 0 ? 1.0 : rho;

            lines = Cv2.HoughLinesP(gray, actualRho, thetaRad, threshold, minLineLength, maxLineGap);

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

            if (lines != null)
            {
                totalLines = lines.Length;
                foreach (var line in lines)
                {
                    Cv2.Line(dst, line.P1, line.P2, color, thickness, LineTypes.AntiAlias);
                }
            }

            return dst;
        }
    }
}

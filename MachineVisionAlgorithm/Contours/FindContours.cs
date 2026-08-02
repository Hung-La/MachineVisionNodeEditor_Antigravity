using OpenCvSharp;

namespace MachineVisionAlgorithm.Contours
{
    public static class FindContours
    {
        public static Mat ApplyFindContours(
            Mat src,
            RetrievalModes mode,
            ContourApproximationModes method,
            Scalar color,
            int thickness,
            out Point[][] contours,
            out HierarchyIndex[] hierarchy)
        {
            contours = System.Array.Empty<Point[]>();
            hierarchy = System.Array.Empty<HierarchyIndex>();

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
            Cv2.FindContours(binaryCopy, out contours, out hierarchy, mode, method);

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

            if (contours != null && contours.Length > 0)
            {
                Cv2.DrawContours(dst, contours, -1, color, thickness);
            }

            return dst;
        }
    }
}

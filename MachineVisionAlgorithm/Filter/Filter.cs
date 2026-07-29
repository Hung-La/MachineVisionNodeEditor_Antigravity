using OpenCvSharp;

namespace MachineVisionAlgorithm.Filter
{
    public static class Filter
    {
        public static Mat ApplyGaussianBlur(Mat src, int kSize, double sigmaX)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            if (kSize % 2 == 0) kSize += 1;
            Mat dst = new Mat();
            Cv2.GaussianBlur(src, dst, new Size(kSize, kSize), sigmaX);
            return dst;
        }

        public static Mat ApplyMedianBlur(Mat src, int kSize)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            if (kSize % 2 == 0) kSize += 1;
            Mat dst = new Mat();
            Cv2.MedianBlur(src, dst, kSize);
            return dst;
        }

        public static Mat ApplyBilateralFilter(Mat src, int d, double sigmaColor, double sigmaSpace)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            Mat dst = new Mat();
            Cv2.BilateralFilter(src, dst, d, sigmaColor, sigmaSpace);
            return dst;
        }

        public static Mat ApplyCanny(Mat src, double threshold1, double threshold2, int apertureSize = 3, bool l2Gradient = false)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            Mat gray = src;
            if (src.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            }
            Mat dst = new Mat();
            Cv2.Canny(gray, dst, threshold1, threshold2, apertureSize, l2Gradient);
            if (gray != src) gray.Dispose();
            return dst;
        }
    }
}

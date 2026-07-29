using OpenCvSharp;

namespace MachineVisionAlgorithm.Binary
{
    public static class Threshold
    {
        public static Mat ApplyThreshold(Mat src, double thresh, double maxVal, ThresholdTypes type)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            Mat gray = src;
            if (src.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            }
            Mat dst = new Mat();
            Cv2.Threshold(gray, dst, thresh, maxVal, type);
            if (gray != src) gray.Dispose();
            return dst;
        }
    }
}

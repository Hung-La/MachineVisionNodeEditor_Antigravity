using OpenCvSharp;

namespace MachineVisionAlgorithm.Morphology
{
    public static class Morphology
    {
        public static Mat ApplyErode(Mat src, MorphShapes shape, int kSize, int iterations = 1)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            if (kSize % 2 == 0) kSize += 1;
            using var element = Cv2.GetStructuringElement(shape, new Size(kSize, kSize));
            Mat dst = new Mat();
            Cv2.Erode(src, dst, element, iterations: iterations);
            return dst;
        }

        public static Mat ApplyDilate(Mat src, MorphShapes shape, int kSize, int iterations = 1)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            if (kSize % 2 == 0) kSize += 1;
            using var element = Cv2.GetStructuringElement(shape, new Size(kSize, kSize));
            Mat dst = new Mat();
            Cv2.Dilate(src, dst, element, iterations: iterations);
            return dst;
        }

        public static Mat ApplyMorphologyEx(Mat src, MorphTypes op, MorphShapes shape, int kSize, int iterations = 1)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();
            if (kSize % 2 == 0) kSize += 1;
            using var element = Cv2.GetStructuringElement(shape, new Size(kSize, kSize));
            Mat dst = new Mat();
            Cv2.MorphologyEx(src, dst, op, element, iterations: iterations);
            return dst;
        }
    }
}

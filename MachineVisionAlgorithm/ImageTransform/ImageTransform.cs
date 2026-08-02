using OpenCvSharp;

namespace MachineVisionAlgorithm.ImageTransform
{
    public static class ImageTransform
    {
        public static Mat ApplyRotate(Mat src, double angle, double scale = 1.0, InterpolationFlags interpolation = InterpolationFlags.Linear, BorderTypes borderMode = BorderTypes.Constant)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();

            Point2f center = new Point2f(src.Width / 2.0f, src.Height / 2.0f);
            using Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, scale);

            Mat dst = new Mat();
            Cv2.WarpAffine(src, dst, rotationMatrix, src.Size(), interpolation, borderMode);
            return dst;
        }

        public static Mat ApplyResize(Mat src, int width, int height, double scaleX = 1.0, double scaleY = 1.0, bool useScaleFactor = false, InterpolationFlags interpolation = InterpolationFlags.Linear)
        {
            if (src == null || src.IsDisposed || src.Empty()) return new Mat();

            Mat dst = new Mat();
            if (useScaleFactor)
            {
                Cv2.Resize(src, dst, new Size(0, 0), scaleX, scaleY, interpolation);
            }
            else
            {
                if (width <= 0) width = src.Width;
                if (height <= 0) height = src.Height;
                Cv2.Resize(src, dst, new Size(width, height), 0, 0, interpolation);
            }
            return dst;
        }
    }
}

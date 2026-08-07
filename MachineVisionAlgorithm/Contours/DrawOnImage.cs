using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace MachineVisionAlgorithm.Contours
{
    public static class DrawOnImage
    {
        /// <summary>
        /// Vẽ dữ liệu hình học (Point[][], LineSegmentPoint[], CircleSegment[], Point2f[], Point[], Mat) lên ảnh nguồn.
        /// </summary>
        public static Mat ApplyDrawOnImage(
            Mat sourceImage, 
            object? drawData,
            Scalar color,
            int thickness = 2,
            bool showText = true)
        {
            if (sourceImage == null || sourceImage.IsDisposed || sourceImage.Empty())
                return new Mat();

            Mat resultImage = sourceImage.Clone();

            if (drawData == null)
                return resultImage;

            switch (drawData)
            {
                case Point[][] contours:
                    DrawContoursData(resultImage, contours, color, thickness, showText);
                    break;

                case List<Point[]> contourList:
                    DrawContoursData(resultImage, contourList.ToArray(), color, thickness, showText);
                    break;

                case LineSegmentPoint[] lines:
                    DrawLinesData(resultImage, lines, color, thickness, showText);
                    break;

                case List<LineSegmentPoint> lineList:
                    DrawLinesData(resultImage, lineList.ToArray(), color, thickness, showText);
                    break;

                case CircleSegment[] circles:
                    DrawCirclesData(resultImage, circles, color, thickness, showText);
                    break;

                case List<CircleSegment> circleList:
                    DrawCirclesData(resultImage, circleList.ToArray(), color, thickness, showText);
                    break;

                case Point2f[] points2f:
                    DrawPoints2fData(resultImage, points2f, color, thickness, showText);
                    break;

                case List<Point2f> point2fList:
                    DrawPoints2fData(resultImage, point2fList.ToArray(), color, thickness, showText);
                    break;

                case Point[] points:
                    DrawPointsData(resultImage, points, color, thickness, showText);
                    break;

                case List<Point> pointList:
                    DrawPointsData(resultImage, pointList.ToArray(), color, thickness, showText);
                    break;

                case Point2f singlePoint2f:
                    DrawPoints2fData(resultImage, new Point2f[] { singlePoint2f }, color, thickness, showText);
                    break;

                case Point singlePoint:
                    DrawPointsData(resultImage, new Point[] { singlePoint }, color, thickness, showText);
                    break;

                case Mat overlayMat:
                    DrawMatOverlay(resultImage, overlayMat, color, thickness);
                    break;
            }

            return resultImage;
        }

        private static void DrawContoursData(Mat resultImage, Point[][] contours, Scalar color, int thickness, bool showText)
        {
            if (contours == null) return;
            for (int i = 0; i < contours.Length; i++)
            {
                if (contours[i] == null || contours[i].Length == 0) continue;

                Cv2.DrawContours(resultImage, contours, contourIdx: i, color, thickness);
                if (showText)
                {
                    Rect boundingBox = Cv2.BoundingRect(contours[i]);
                    int textY = Math.Max(15, boundingBox.Y - 5);
                    Cv2.PutText(resultImage, $"#{i + 1}", new Point(boundingBox.X, textY),
                                fontFace: HersheyFonts.HersheySimplex, fontScale: 0.6,
                                color, Math.Max(1, thickness / 2));
                }
            }
        }

        private static void DrawLinesData(Mat resultImage, LineSegmentPoint[] lines, Scalar color, int thickness, bool showText)
        {
            if (lines == null) return;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                Cv2.Line(resultImage, line.P1, line.P2, color, thickness, LineTypes.AntiAlias);

                if (showText)
                {
                    Point midPoint = new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
                    int textY = Math.Max(15, midPoint.Y - 5);
                    Cv2.PutText(resultImage, $"L#{i + 1}", new Point(midPoint.X, textY),
                                fontFace: HersheyFonts.HersheySimplex, fontScale: 0.5,
                                color, Math.Max(1, thickness / 2));
                }
            }
        }

        private static void DrawCirclesData(Mat resultImage, CircleSegment[] circles, Scalar color, int thickness, bool showText)
        {
            if (circles == null) return;
            for (int i = 0; i < circles.Length; i++)
            {
                var circle = circles[i];
                Point center = new Point((int)Math.Round(circle.Center.X), (int)Math.Round(circle.Center.Y));
                int radius = (int)Math.Round(circle.Radius);

                Cv2.Circle(resultImage, center, radius, color, thickness, LineTypes.AntiAlias);
                Cv2.Circle(resultImage, center, 2, color, -1, LineTypes.AntiAlias);

                if (showText)
                {
                    int textY = Math.Max(15, center.Y - radius - 5);
                    Cv2.PutText(resultImage, $"C#{i + 1}", new Point(Math.Max(5, center.X - radius), textY),
                                fontFace: HersheyFonts.HersheySimplex, fontScale: 0.5,
                                color, Math.Max(1, thickness / 2));
                }
            }
        }

        private static void DrawPoints2fData(Mat resultImage, Point2f[] points, Scalar color, int thickness, bool showText)
        {
            if (points == null) return;
            for (int i = 0; i < points.Length; i++)
            {
                Point pt = new Point((int)Math.Round(points[i].X), (int)Math.Round(points[i].Y));
                int radius = Math.Max(3, thickness * 2);
                Cv2.Circle(resultImage, pt, radius, color, -1, LineTypes.AntiAlias);
                Cv2.DrawMarker(resultImage, pt, color, MarkerTypes.Cross, radius * 2, thickness);

                if (showText)
                {
                    int textY = Math.Max(15, pt.Y - radius - 5);
                    Cv2.PutText(resultImage, $"P#{i + 1}", new Point(pt.X + 5, textY),
                                fontFace: HersheyFonts.HersheySimplex, fontScale: 0.5,
                                color, Math.Max(1, thickness / 2));
                }
            }
        }

        private static void DrawPointsData(Mat resultImage, Point[] points, Scalar color, int thickness, bool showText)
        {
            if (points == null) return;
            for (int i = 0; i < points.Length; i++)
            {
                Point pt = points[i];
                int radius = Math.Max(3, thickness * 2);
                Cv2.Circle(resultImage, pt, radius, color, -1, LineTypes.AntiAlias);
                Cv2.DrawMarker(resultImage, pt, color, MarkerTypes.Cross, radius * 2, thickness);

                if (showText)
                {
                    int textY = Math.Max(15, pt.Y - radius - 5);
                    Cv2.PutText(resultImage, $"P#{i + 1}", new Point(pt.X + 5, textY),
                                fontFace: HersheyFonts.HersheySimplex, fontScale: 0.5,
                                color, Math.Max(1, thickness / 2));
                }
            }
        }

        private static void DrawMatOverlay(Mat resultImage, Mat overlayMat, Scalar color, int thickness)
        {
            if (overlayMat == null || overlayMat.IsDisposed || overlayMat.Empty()) return;

            Mat src = overlayMat;
            if (src.Size() != resultImage.Size())
            {
                using Mat resized = new Mat();
                Cv2.Resize(src, resized, resultImage.Size());
                ApplyBlend(resultImage, resized);
            }
            else
            {
                ApplyBlend(resultImage, src);
            }
        }

        private static void ApplyBlend(Mat dst, Mat src)
        {
            if (src.Channels() == 1)
            {
                using Mat colorMask = new Mat();
                Cv2.CvtColor(src, colorMask, ColorConversionCodes.GRAY2BGR);
                Cv2.AddWeighted(dst, 0.7, colorMask, 0.3, 0, dst);
            }
            else
            {
                Cv2.AddWeighted(dst, 0.7, src, 0.3, 0, dst);
            }
        }
    }
}

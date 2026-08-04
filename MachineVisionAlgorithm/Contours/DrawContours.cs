using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionAlgorithm.Contours
{
    public static class DrawContours
    {
        public static Mat ApplyDrawContours(
            Mat sourceImage, 
            Point[][] contours,
            Scalar color,
            int thickness = 2,
            bool showText = true)
        {
            Mat resultImage = new Mat();
            if (sourceImage != null)
            {
                resultImage = sourceImage.Clone();
                if (contours != null)
                {
                    for (int i = 0; i < contours.Length; i++)
                    {
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

                    return resultImage;
                }
            }

            return resultImage;
        }
    }
}

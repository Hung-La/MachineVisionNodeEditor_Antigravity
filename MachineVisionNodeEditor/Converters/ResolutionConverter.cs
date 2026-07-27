using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;

namespace MachineVisionNodeEditor.Converters
{
    public class ResolutionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string filePath)
            {
                if (File.Exists(filePath))
                {
                    var bitmap = Cv2.ImRead(filePath);

                    switch (parameter?.ToString())
                    {
                        case "PixelWidth":
                            return bitmap.Width;

                        case "PixelHeight":
                            return bitmap.Height;

                        case "Width":
                            return bitmap.Width;

                        case "Height":
                            return bitmap.Height;

                        case "Resolution":
                            return $"Image Resolution: {bitmap.Width} x {bitmap.Height}";
                    }
                }
            }
            else if (value is Mat image)
            {
                if (image != null)
                {
                    switch (parameter?.ToString())
                    {
                        case "PixelWidth":
                            return image.Width;

                        case "PixelHeight":
                            return image.Height;

                        case "Width":
                            return image.Width;

                        case "Height":
                            return image.Height;

                        case "Resolution":
                            return $"Image Resolution: {image.Width} x {image.Height}";
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

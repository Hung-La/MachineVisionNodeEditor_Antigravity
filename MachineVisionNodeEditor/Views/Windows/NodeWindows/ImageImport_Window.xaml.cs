using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace MachineVisionNodeEditor.Views.Windows.NodeWindows
{
    /// <summary>
    /// Interaction logic for ImageImport_Window.xaml
    /// </summary>
    public partial class ImageImport_Window : Window
    {

        private bool isPanning;

        private Point start;
        public ImageImport_Window()
        {
            InitializeComponent();

        }


        private void ImageImportWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isPanning = true;

            start = e.GetPosition(this);

            Image.CaptureMouse();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var pixelCoordinate = e.GetPosition(Image);
            TextBlock_PixelCoordinate.Text = $"X: {pixelCoordinate.X:0}, Y: {pixelCoordinate.Y:0} (pixel)";

            if (!isPanning)
                return;

            Point current = e.GetPosition(this);
            Vector delta = current - start;
            //translate.X += delta.X;
            //translate.Y += delta.Y;

            translate.X = Math.Max(translate.X + delta.X, 0);
            translate.Y = Math.Max(translate.Y + delta.Y, 0);

            start = current;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isPanning = false;
            Image.ReleaseMouseCapture();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? 1.1 : 0.9;

            scale.ScaleX *= zoom;
            scale.ScaleY *= zoom;

        }

    }
}

using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace MachineVisionNodeEditor.Views.ImageViewer
{
    /// <summary>
    /// Interaction logic for ImageViewerControl.xaml
    /// </summary>
    public partial class ImageViewerControl : UserControl
    {
        private bool isPanning;
        private Point start;
        public Mat ViewImage
        {
            get { return (Mat)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(ViewImage), typeof(Mat), typeof(ImageViewerControl), new PropertyMetadata(null));


        public Brush PenBrush
        {
            get { return (Brush)GetValue(PenBrushProperty); }
            set { SetValue(PenBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PenBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PenBrushProperty =
            DependencyProperty.Register(nameof(PenBrush), typeof(Brush), typeof(ImageViewerControl), new PropertyMetadata(Brushes.DimGray));


        public double PenThickness
        {
            get { return (double)GetValue(PenThicknessProperty); }
            set { SetValue(PenThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PenThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PenThicknessProperty =
            DependencyProperty.Register(nameof(PenThickness), typeof(double), typeof(ImageViewerControl), new PropertyMetadata(1.5));




        public ImageViewerControl()
        {
            InitializeComponent();
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
            //TextBlock_PixelCoordinate.Text = $"X: {pixelCoordinate.X:0}, Y: {pixelCoordinate.Y:0} (pixel)";

            Window window = UIHelper.GetWindowParent(this) as Window;
            NodeWindow nodeWindow = (NodeWindow)window;
            nodeWindow.TextBlock_PixelCoordinate.Text = $"X: {pixelCoordinate.X:0}, Y: {pixelCoordinate.Y:0} (pixel)";

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

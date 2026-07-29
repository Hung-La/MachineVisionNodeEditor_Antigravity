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
using Rect = System.Windows.Rect;
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

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(ViewImage), typeof(Mat), typeof(ImageViewerControl), new PropertyMetadata(null));

        public bool IsGridVisible
        {
            get { return (bool)GetValue(IsGridVisibleProperty); }
            set { SetValue(IsGridVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsGridVisibleProperty =
            DependencyProperty.Register(nameof(IsGridVisible), typeof(bool), typeof(ImageViewerControl),
                new PropertyMetadata(true, OnGridPropertyChanged));

        public double GridSpacing
        {
            get { return (double)GetValue(GridSpacingProperty); }
            set { SetValue(GridSpacingProperty, value); }
        }

        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register(nameof(GridSpacing), typeof(double), typeof(ImageViewerControl),
                new PropertyMetadata(100.0, OnGridPropertyChanged));

        public Brush PenBrush
        {
            get { return (Brush)GetValue(PenBrushProperty); }
            set { SetValue(PenBrushProperty, value); }
        }

        public static readonly DependencyProperty PenBrushProperty =
            DependencyProperty.Register(nameof(PenBrush), typeof(Brush), typeof(ImageViewerControl),
                new PropertyMetadata(Brushes.DimGray, OnGridPropertyChanged));

        public double PenThickness
        {
            get { return (double)GetValue(PenThicknessProperty); }
            set { SetValue(PenThicknessProperty, value); }
        }

        public static readonly DependencyProperty PenThicknessProperty =
            DependencyProperty.Register(nameof(PenThickness), typeof(double), typeof(ImageViewerControl),
                new PropertyMetadata(1.5, OnGridPropertyChanged));

        private static void OnGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerControl control)
            {
                control.UpdateGrid();
            }
        }

        public ImageViewerControl()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateGrid();
        }

        public void UpdateGrid()
        {
            if (OverlayCanvas == null) return;

            if (!IsGridVisible)
            {
                OverlayCanvas.Visibility = Visibility.Collapsed;
                return;
            }

            OverlayCanvas.Visibility = Visibility.Visible;
            double spacing = Math.Max(1.0, GridSpacing);

            var drawingGroup = new DrawingGroup();

            // Background geometry
            var bgDrawing = new GeometryDrawing(Brushes.Transparent, null, new RectangleGeometry(new Rect(0, 0, spacing, spacing)));
            drawingGroup.Children.Add(bgDrawing);

            // Grid Pen
            var pen = new Pen(PenBrush ?? Brushes.DimGray, PenThickness)
            {
                DashStyle = new DashStyle(new double[] { 1.5, 1.5 }, 0)
            };

            var lineGroup = new GeometryGroup();
            lineGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(0, spacing)));
            lineGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(spacing, 0)));

            var gridDrawing = new GeometryDrawing(Brushes.Transparent, pen, lineGroup);
            drawingGroup.Children.Add(gridDrawing);

            var drawingBrush = new DrawingBrush
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, spacing, spacing),
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = drawingGroup
            };

            OverlayCanvas.Background = drawingBrush;
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

            Window window = UIHelper.GetWindowParent(this) as Window;
            if (window is NodeWindow nodeWindow)
            {
                nodeWindow.TextBlock_PixelCoordinate.Text = $"X: {pixelCoordinate.X:0}, Y: {pixelCoordinate.Y:0} (pixel)";
            }

            if (!isPanning)
                return;

            Point current = e.GetPosition(this);
            Vector delta = current - start;

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

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
            DependencyProperty.Register(nameof(ViewImage), typeof(Mat), typeof(ImageViewerControl),
                new PropertyMetadata(null, OnViewImageChanged));

        private static void OnViewImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerControl control)
            {
                control.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
                {
                    control.FitToScreen();
                }));
            }
        }

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
            Loaded += (s, e) =>
            {
                UpdateGrid();
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
                {
                    FitToScreen();
                }));
            };
            SizeChanged += (s, e) =>
            {
                FitToScreen();
            };
        }

        public void FitToScreen()
        {
            if (ViewImage == null || ViewImage.IsDisposed || ViewImage.Width <= 0 || ViewImage.Height <= 0)
                return;

            double containerWidth = ActualWidth > 0 ? ActualWidth : 800;
            double containerHeight = ActualHeight > 0 ? ActualHeight : 600;

            double availW = Math.Max(containerWidth - 20, 50);
            double availH = Math.Max(containerHeight - 20, 50);

            double scaleX = availW / ViewImage.Width;
            double scaleY = availH / ViewImage.Height;

            double fitScale = Math.Min(scaleX, scaleY);
            if (fitScale <= 0 || double.IsNaN(fitScale) || double.IsInfinity(fitScale))
                fitScale = 1.0;

            scale.CenterX = 0;
            scale.CenterY = 0;

            scale.ScaleX = fitScale;
            scale.ScaleY = fitScale;

            translate.X = (containerWidth - ViewImage.Width * fitScale) / 2.0;
            translate.Y = (containerHeight - ViewImage.Height * fitScale) / 2.0;

            if (_activeCropModel != null)
            {
                CropRoiOverlay.ZoomScale = fitScale;
            }
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
            if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
            {
                FitToScreen();
                e.Handled = true;
                return;
            }

            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                isPanning = true;
                start = e.GetPosition(this);
                Image.CaptureMouse();
            }
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

            translate.X = translate.X + delta.X;
            translate.Y = translate.Y + delta.Y;

            start = current;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                isPanning = false;
                Image.ReleaseMouseCapture();
            }
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewImage == null || ViewImage.IsDisposed) return;

            double zoom = e.Delta > 0 ? 1.1 : 0.9;
            Point mousePos = e.GetPosition(this);

            double mouseInImageX = (mousePos.X - translate.X) / scale.ScaleX;
            double mouseInImageY = (mousePos.Y - translate.Y) / scale.ScaleY;

            scale.CenterX = 0;
            scale.CenterY = 0;

            scale.ScaleX *= zoom;
            scale.ScaleY *= zoom;

            translate.X = mousePos.X - mouseInImageX * scale.ScaleX;
            translate.Y = mousePos.Y - mouseInImageY * scale.ScaleY;

            if (_activeCropModel != null)
            {
                CropRoiOverlay.ZoomScale = scale.ScaleX;
            }
        }

        private Models.NodePropertyModels.ImageCrop_NodePropertyModel? _activeCropModel;

        public void EnableCropMode(Models.NodePropertyModels.ImageCrop_NodePropertyModel? cropModel)
        {
            _activeCropModel = cropModel;
            if (cropModel == null || ViewImage == null || ViewImage.IsDisposed || ViewImage.Width <= 0 || ViewImage.Height <= 0)
            {
                CropRoiOverlay.Visibility = Visibility.Collapsed;
                return;
            }

            CropRoiOverlay.ImageWidth = ViewImage.Width;
            CropRoiOverlay.ImageHeight = ViewImage.Height;
            CropRoiOverlay.ZoomScale = scale.ScaleX;

            BindingOperations.ClearBinding(CropRoiOverlay, CropRoiControl.CropXProperty);
            BindingOperations.ClearBinding(CropRoiOverlay, CropRoiControl.CropYProperty);
            BindingOperations.ClearBinding(CropRoiOverlay, CropRoiControl.CropWidthProperty);
            BindingOperations.ClearBinding(CropRoiOverlay, CropRoiControl.CropHeightProperty);

            CropRoiOverlay.SetBinding(CropRoiControl.CropXProperty, new Binding(nameof(Models.NodePropertyModels.ImageCrop_NodePropertyModel.CropX))
            {
                Source = cropModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            CropRoiOverlay.SetBinding(CropRoiControl.CropYProperty, new Binding(nameof(Models.NodePropertyModels.ImageCrop_NodePropertyModel.CropY))
            {
                Source = cropModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            CropRoiOverlay.SetBinding(CropRoiControl.CropWidthProperty, new Binding(nameof(Models.NodePropertyModels.ImageCrop_NodePropertyModel.CropWidth))
            {
                Source = cropModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            CropRoiOverlay.SetBinding(CropRoiControl.CropHeightProperty, new Binding(nameof(Models.NodePropertyModels.ImageCrop_NodePropertyModel.CropHeight))
            {
                Source = cropModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            CropRoiOverlay.Visibility = Visibility.Visible;
            CropRoiOverlay.UpdateLayoutGeometry();
        }
    }
}

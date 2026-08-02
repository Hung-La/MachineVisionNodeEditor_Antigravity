using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MachineVisionNodeEditor.Views.ImageViewer
{
    public partial class CropRoiControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty CropXProperty =
            DependencyProperty.Register(nameof(CropX), typeof(int), typeof(CropRoiControl),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGeometryPropertyChanged));

        public static readonly DependencyProperty CropYProperty =
            DependencyProperty.Register(nameof(CropY), typeof(int), typeof(CropRoiControl),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGeometryPropertyChanged));

        public static readonly DependencyProperty CropWidthProperty =
            DependencyProperty.Register(nameof(CropWidth), typeof(int), typeof(CropRoiControl),
                new FrameworkPropertyMetadata(200, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGeometryPropertyChanged));

        public static readonly DependencyProperty CropHeightProperty =
            DependencyProperty.Register(nameof(CropHeight), typeof(int), typeof(CropRoiControl),
                new FrameworkPropertyMetadata(200, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGeometryPropertyChanged));

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(int), typeof(CropRoiControl),
                new PropertyMetadata(0, OnGeometryPropertyChanged));

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(int), typeof(CropRoiControl),
                new PropertyMetadata(0, OnGeometryPropertyChanged));

        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register(nameof(ZoomScale), typeof(double), typeof(CropRoiControl),
                new PropertyMetadata(1.0, OnGeometryPropertyChanged));

        public int CropX
        {
            get => (int)GetValue(CropXProperty);
            set => SetValue(CropXProperty, value);
        }

        public int CropY
        {
            get => (int)GetValue(CropYProperty);
            set => SetValue(CropYProperty, value);
        }

        public int CropWidth
        {
            get => (int)GetValue(CropWidthProperty);
            set => SetValue(CropWidthProperty, value);
        }

        public int CropHeight
        {
            get => (int)GetValue(CropHeightProperty);
            set => SetValue(CropHeightProperty, value);
        }

        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public double ZoomScale
        {
            get => (double)GetValue(ZoomScaleProperty);
            set => SetValue(ZoomScaleProperty, value);
        }

        #endregion

        public CropRoiControl()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateLayoutGeometry();
        }

        private static void OnGeometryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CropRoiControl control)
            {
                control.UpdateLayoutGeometry();
            }
        }

        public void UpdateLayoutGeometry()
        {
            if (ImageWidth <= 0 || ImageHeight <= 0)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            Visibility = Visibility.Visible;

            int x = Math.Clamp(CropX, 0, Math.Max(0, ImageWidth - 1));
            int y = Math.Clamp(CropY, 0, Math.Max(0, ImageHeight - 1));
            int w = Math.Clamp(CropWidth, 1, Math.Max(1, ImageWidth - x));
            int h = Math.Clamp(CropHeight, 1, Math.Max(1, ImageHeight - y));

            RootCanvas.Width = ImageWidth;
            RootCanvas.Height = ImageHeight;

            // Dark dimming overlay around crop box
            var outerRect = new RectangleGeometry(new Rect(0, 0, ImageWidth, ImageHeight));
            var innerRect = new RectangleGeometry(new Rect(x, y, w, h));
            DimmingPath.Data = new CombinedGeometry(GeometryCombineMode.Exclude, outerRect, innerRect);

            // Position crop box container
            Canvas.SetLeft(CropBoxCanvas, x);
            Canvas.SetTop(CropBoxCanvas, y);
            CropBoxCanvas.Width = w;
            CropBoxCanvas.Height = h;

            CropBorder.Width = w;
            CropBorder.Height = h;

            ThumbMove.Width = w;
            ThumbMove.Height = h;

            // Rule of thirds grid
            var gridGeo = new GeometryGroup();
            gridGeo.Children.Add(new LineGeometry(new Point(w / 3.0, 0), new Point(w / 3.0, h)));
            gridGeo.Children.Add(new LineGeometry(new Point(2.0 * w / 3.0, 0), new Point(2.0 * w / 3.0, h)));
            gridGeo.Children.Add(new LineGeometry(new Point(0, h / 3.0), new Point(w, h / 3.0)));
            gridGeo.Children.Add(new LineGeometry(new Point(0, 2.0 * h / 3.0), new Point(w, 2.0 * h / 3.0)));
            GridLines.Data = gridGeo;

            InfoText.Text = $"X: {x}, Y: {y} | {w} × {h} px";

            // Float badge above top-left
            Canvas.SetLeft(InfoBadge, 0);
            double badgeTop = y >= 30 ? -28 : 4;
            Canvas.SetTop(InfoBadge, badgeTop);

            // Handle sizing dynamically adjusted by zoom scale
            double scaleFactor = Math.Max(0.05, ZoomScale);
            double handleSize = 10.0 / scaleFactor;
            double halfH = handleSize / 2.0;

            void PositionThumb(Thumb thumb, double left, double top)
            {
                thumb.Width = handleSize;
                thumb.Height = handleSize;
                Canvas.SetLeft(thumb, left - halfH);
                Canvas.SetTop(thumb, top - halfH);
            }

            PositionThumb(ThumbTL, 0, 0);
            PositionThumb(ThumbT, w / 2.0, 0);
            PositionThumb(ThumbTR, w, 0);
            PositionThumb(ThumbR, w, h / 2.0);
            PositionThumb(ThumbBR, w, h);
            PositionThumb(ThumbB, w / 2.0, h);
            PositionThumb(ThumbBL, 0, h);
            PositionThumb(ThumbL, 0, h / 2.0);
        }

        #region Drag Event Handlers

        private void ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ImageWidth <= 0 || ImageHeight <= 0) return;

            int newX = Math.Clamp((int)Math.Round(CropX + e.HorizontalChange), 0, Math.Max(0, ImageWidth - CropWidth));
            int newY = Math.Clamp((int)Math.Round(CropY + e.VerticalChange), 0, Math.Max(0, ImageHeight - CropHeight));

            CropX = newX;
            CropY = newY;
        }

        private void ThumbTL_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int maxRight = CropX + CropWidth - 1;
            int maxBottom = CropY + CropHeight - 1;

            int newX = Math.Clamp((int)Math.Round(CropX + e.HorizontalChange), 0, maxRight);
            int newY = Math.Clamp((int)Math.Round(CropY + e.VerticalChange), 0, maxBottom);

            int newW = maxRight - newX + 1;
            int newH = maxBottom - newY + 1;

            CropX = newX;
            CropY = newY;
            CropWidth = newW;
            CropHeight = newH;
        }

        private void ThumbT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int maxBottom = CropY + CropHeight - 1;
            int newY = Math.Clamp((int)Math.Round(CropY + e.VerticalChange), 0, maxBottom);
            int newH = maxBottom - newY + 1;

            CropY = newY;
            CropHeight = newH;
        }

        private void ThumbTR_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int maxBottom = CropY + CropHeight - 1;
            int newY = Math.Clamp((int)Math.Round(CropY + e.VerticalChange), 0, maxBottom);
            int newH = maxBottom - newY + 1;

            int newW = Math.Clamp((int)Math.Round(CropWidth + e.HorizontalChange), 1, Math.Max(1, ImageWidth - CropX));

            CropY = newY;
            CropHeight = newH;
            CropWidth = newW;
        }

        private void ThumbR_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int newW = Math.Clamp((int)Math.Round(CropWidth + e.HorizontalChange), 1, Math.Max(1, ImageWidth - CropX));
            CropWidth = newW;
        }

        private void ThumbBR_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int newW = Math.Clamp((int)Math.Round(CropWidth + e.HorizontalChange), 1, Math.Max(1, ImageWidth - CropX));
            int newH = Math.Clamp((int)Math.Round(CropHeight + e.VerticalChange), 1, Math.Max(1, ImageHeight - CropY));

            CropWidth = newW;
            CropHeight = newH;
        }

        private void ThumbB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int newH = Math.Clamp((int)Math.Round(CropHeight + e.VerticalChange), 1, Math.Max(1, ImageHeight - CropY));
            CropHeight = newH;
        }

        private void ThumbBL_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int maxRight = CropX + CropWidth - 1;
            int newX = Math.Clamp((int)Math.Round(CropX + e.HorizontalChange), 0, maxRight);
            int newW = maxRight - newX + 1;

            int newH = Math.Clamp((int)Math.Round(CropHeight + e.VerticalChange), 1, Math.Max(1, ImageHeight - CropX));

            CropX = newX;
            CropWidth = newW;
            CropHeight = newH;
        }

        private void ThumbL_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int maxRight = CropX + CropWidth - 1;
            int newX = Math.Clamp((int)Math.Round(CropX + e.HorizontalChange), 0, maxRight);
            int newW = maxRight - newX + 1;

            CropX = newX;
            CropWidth = newW;
        }

        #endregion
    }
}

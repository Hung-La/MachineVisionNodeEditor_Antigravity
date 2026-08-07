using MachineVisionNodeEditor.Adorners;
using MachineVisionNodeEditor.Builders;
using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Nodes;
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

namespace MachineVisionNodeEditor.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;
        private static readonly object _lock = new object();

        public static MainWindow Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new MainWindow();

                    }
                    return instance;
                }
            }

        }

        public Window_MainWindowViewModel Window_MainWindowViewModel { get; } = new Window_MainWindowViewModel();


        private ZoomSliderAdorner _adorner;
        private AdornerLayer _layer;

        // ── Panning ──────────────────────────────────────────────────────
        private bool _isPanning;
        private Point _lastMousePosition;

        // ── Selection Box (rubber-band) ───────────────────────────────────
        private bool _isSelecting;
        private Point _selectionStart;  // toạ độ canvas khi bắt đầu kéo
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Window_MainWindowViewModel;
            instance = this;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var cursorCoordinate = e.GetPosition(MainCanvas);
            TextBlock_CursorCoordinate.Text = $"(X: {cursorCoordinate.X:0}, Y: {cursorCoordinate.Y:0})";

            // ── Panning ──
            if (_isPanning)
            {
                Point current = e.GetPosition(this);
                Vector delta = current - _lastMousePosition;

                Window_MainWindowViewModel.OffsetX += delta.X;
                Window_MainWindowViewModel.OffsetY += delta.Y;

                _lastMousePosition = current;
                return;
            }

            // ── Kéo dây nối ──
            if (e.LeftButton == MouseButtonState.Pressed && NodeControl.DraggingPort != null)
            {
                var startEl = NodeControl.DraggingPort.View;
                if (startEl != null)
                    PreviewPath.Data = MakeBezier(
                        UIHelper.GetCenter(startEl, MainCanvas),
                        cursorCoordinate);
                e.Handled = true;
                return;
            }

            // ── Vẽ selection box ──
            if (_isSelecting && e.LeftButton == MouseButtonState.Pressed)
            {
                double x = Math.Min(_selectionStart.X, cursorCoordinate.X);
                double y = Math.Min(_selectionStart.Y, cursorCoordinate.Y);
                double w = Math.Abs(cursorCoordinate.X - _selectionStart.X);
                double h = Math.Abs(cursorCoordinate.Y - _selectionStart.Y);

                Canvas.SetLeft(SelectionBox, x);
                Canvas.SetTop(SelectionBox, y);
                SelectionBox.Width = w;
                SelectionBox.Height = h;
                e.Handled = true;
            }
        }
        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ── Kết thúc selection box ──
                if (_isSelecting && e.ChangedButton == MouseButton.Left)
                {
                    _isSelecting = false;
                    SelectionBox.Visibility = Visibility.Collapsed;

                    double x = Canvas.GetLeft(SelectionBox);
                    double y = Canvas.GetTop(SelectionBox);
                    var rect = new Rect(x, y, SelectionBox.Width, SelectionBox.Height);

                    // Chỉ select khi kéo đủ lớn (tránh nhầm với click đơn)
                    if (rect.Width > 5 || rect.Height > 5)
                        Window_MainWindowViewModel.SelectInRect(rect);

                    Mouse.Capture(null);
                    e.Handled = true;
                    return;
                }

                var dragging = NodeControl.DraggingPort;
                if (dragging != null)
                {

                    // Hit-test toàn bộ visual tree tại vị trí thả chuột
                    var pos = e.GetPosition(MainCanvas);
                    PortModel? target = UIHelper.HitTestPort(pos);

                    // ── Kết thúc kéo dây ──
                    if (target != null && UIHelper.IsValidConnection(dragging, target))
                    {
                        var fromEl = UIHelper.FindPortElement(dragging);
                        var toEl = UIHelper.FindPortElement(target);

                        if (fromEl != null && toEl != null)
                        {

                            // Connection được tạo trong Node_PortView.Button_PreviewMouseLeftButtonUp
                            //Point startPt = UIHelper.GetCenter(fromEl, MainCanvas);
                            //Point endPt = UIHelper.GetCenter(toEl, MainCanvas);

                        }
                    }

                    // ── Kết thúc panning ──
                    if (e.ChangedButton == MouseButton.Middle)
                    {
                        _isPanning = false;
                        Mouse.Capture(null);
                        MainCanvas.Cursor = Cursors.Arrow;
                    }

                }
            }
            finally
            {

                if (e.ChangedButton == MouseButton.Middle)
                {
                    _isPanning = false;

                    Mouse.Capture(null);

                    MainCanvas.Cursor = Cursors.Arrow;
                }

                NodeControl.ClearDraggingPort();
                PreviewPath.Data = null;
                e.Handled = true;
            }
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window_MainWindowViewModel.SelectionService.Clear();

            if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
            {
                FitToScreen();
                e.Handled = true;
                return;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;

                _lastMousePosition = e.GetPosition(this);

                Mouse.Capture(MainCanvas);

                MainCanvas.Cursor = Cursors.SizeAll;
            }

            // Left button trên canvas trống → bắt đầu vẽ selection box
            if (e.ChangedButton == MouseButton.Left && e.OriginalSource is Canvas)
            {
                Window_MainWindowViewModel.ClearAllSelections();

                _isSelecting = true;
                _selectionStart = e.GetPosition(MainCanvas);

                Canvas.SetLeft(SelectionBox, _selectionStart.X);
                Canvas.SetTop(SelectionBox, _selectionStart.Y);
                SelectionBox.Width = 0;
                SelectionBox.Height = 0;
                SelectionBox.Visibility = Visibility.Visible;

                Mouse.Capture(MainCanvas);
                e.Handled = true;
            }
        }

        private static PathGeometry MakeBezier(Point p1, Point p2)
        {
            double dx = Math.Abs(p2.X - p1.X) * 0.6;
            var fig = new PathFigure { StartPoint = p1, IsFilled = false };
            fig.Segments.Add(new BezierSegment(
                new Point(p1.X + dx, p1.Y),
                new Point(p2.X - dx, p2.Y),
                p2, isStroked: true));
            return new PathGeometry(new[] { fig });
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Window_MainWindowViewModel.DeleteSelection();
            }

            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Window_MainWindowViewModel.UndoRedoService.Undo();
            }

            if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Window_MainWindowViewModel.UndoRedoService.Redo();
            }

            if (e.Key == Key.F5 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Window_MainWindowViewModel.PipelineExecuteCommand.CanExecute(null))
                {
                    Window_MainWindowViewModel.PipelineExecuteCommand.Execute(null);
                }
            }

            // ── Copy / Cut / Paste ──
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Window_MainWindowViewModel.CopySelection();
                e.Handled = true;
            }

            if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Window_MainWindowViewModel.CutSelection();
                e.Handled = true;
            }

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Window_MainWindowViewModel.PasteSelection();
                e.Handled = true;
            }

            // ── File operations ──
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Window_MainWindowViewModel.NewPipelineCommand.CanExecute(null))
                    Window_MainWindowViewModel.NewPipelineCommand.Execute(null);
                e.Handled = true;
            }

            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Window_MainWindowViewModel.OpenPipelineCommand.CanExecute(null))
                    Window_MainWindowViewModel.OpenPipelineCommand.Execute(null);
                e.Handled = true;
            }

            if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (Window_MainWindowViewModel.SaveAsPipelineCommand.CanExecute(null))
                    Window_MainWindowViewModel.SaveAsPipelineCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Window_MainWindowViewModel.SavePipelineCommand.CanExecute(null))
                    Window_MainWindowViewModel.SavePipelineCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Không giữ Ctrl thì để ScrollViewer xử lý bình thường
            //if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            //{
            //    e.Handled = true;
            //    return;
            //}


            double zoom = Window_MainWindowViewModel.ZoomFactor;

            zoom += e.Delta > 0 ? 0.1 : -0.1;

            zoom = Math.Clamp(zoom, 0.2, 3);

            Window_MainWindowViewModel.ZoomFactor = zoom;

            // Đã xử lý sự kiện nên ScrollViewer sẽ không cuộn
            e.Handled = true;
        }

        private void FitToScreen()
        {
            if (!Window_MainWindowViewModel.Nodes.Any())
                return;

            const double margin = 50;

            //=========================
            // Bounding Box
            //=========================

            double minX = Window_MainWindowViewModel.Nodes.Min(n => n.NodeModel.X);

            double minY = Window_MainWindowViewModel.Nodes.Min(n => n.NodeModel.Y);

            double maxX = Window_MainWindowViewModel.Nodes.Max(n => n.NodeModel.X + 250);

            double maxY = Window_MainWindowViewModel.Nodes.Max(n => n.NodeModel.Y + 250);

            double diagramWidth = maxX - minX;
            double diagramHeight = maxY - minY;

            if (diagramWidth <= 0 || diagramHeight <= 0)
                return;

            //=========================
            // Viewport
            //=========================

            if (MainCanvas.Parent is not FrameworkElement viewport)
                return;

            double viewportWidth = viewport.ActualWidth;
            double viewportHeight = viewport.ActualHeight;

            //=========================
            // Zoom
            //=========================

            double zoomX = (viewportWidth - margin * 2) / diagramWidth;

            double zoomY = (viewportHeight - margin * 2) / diagramHeight;

            double zoom = Math.Min(zoomX, zoomY);

            zoom = Math.Clamp(zoom, 0.2, 3.0);

            Window_MainWindowViewModel.ZoomFactor = zoom;

            //=========================
            // Center
            //=========================

            double centerX = (minX + maxX) / 2;
            double centerY = (minY + maxY) / 2;

            Window_MainWindowViewModel.OffsetX =
                viewportWidth / 2 - centerX * zoom;

            Window_MainWindowViewModel.OffsetY =
                viewportHeight / 2 - centerY * zoom;
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in OwnedWindows)
            {
                window.Close();
            }
        }
    }
}
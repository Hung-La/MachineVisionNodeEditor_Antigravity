using MachineVisionNodeEditor.Builders;
using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Factories;
using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Registries;
using MachineVisionNodeEditor.Resources.Themes;
using MachineVisionNodeEditor.Services;
using MachineVisionNodeEditor.Services.NodeServies;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using MachineVisionNodeEditor.Views.Windows;
using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.WindowViewModels
{
    public class Window_MainWindowViewModel : BaseViewModel
    {
        #region Fields
        private bool isChecked_ToggleTheme;
        private Node_ConnectionViewModel _pendingConnection;
        private bool _isDraggingConnection;

        private bool _isPipelineExecutedSuccess;
        private string _pipelineExecutionStatusText = string.Empty;
        private bool _isPipelineStatusVisible;

        public ClipboardService ClipboardService { get; } = new();

        /// <summary>Đường dẫn file project hiện tại (null nếu chưa lưu).</summary>
        private string? _currentFilePath;

        /// <summary>Cờ đánh dấu diagram đã thay đổi kể từ lần lưu cuối.</summary>
        private bool _isDirty;

        /// <summary>Hằng số cho file filter dialog.</summary>
        private const string FileFilter = "Machine Vision Node Editor (*.mvne)|*.mvne|All files (*.*)|*.*";
        #endregion

        #region Properties

        /// <summary>Đường dẫn file project hiện tại.</summary>
        public string? CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                SetField(ref _currentFilePath, value);
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        /// <summary>Cờ đánh dấu diagram đã thay đổi kể từ lần lưu cuối.</summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                SetField(ref _isDirty, value);
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        /// <summary>Tiêu đề cửa sổ hiển thị tên file và trạng thái thay đổi.</summary>
        public string WindowTitle
        {
            get
            {
                var fileName = CurrentFilePath != null
                    ? Path.GetFileName(CurrentFilePath)
                    : "Untitled";
                var dirty = IsDirty ? " *" : "";
                return $"{fileName}{dirty} - Machine Vision Node Editor";
            }
        }

        public bool IsPipelineExecutedSuccess
        {
            get => _isPipelineExecutedSuccess;
            set => SetField(ref _isPipelineExecutedSuccess, value);
        }

        public string PipelineExecutionStatusText
        {
            get => _pipelineExecutionStatusText;
            set => SetField(ref _pipelineExecutionStatusText, value);
        }

        public bool IsPipelineStatusVisible
        {
            get => _isPipelineStatusVisible;
            set => SetField(ref _isPipelineStatusVisible, value);
        }

        public bool IsChecked_ToggleTheme { get => isChecked_ToggleTheme; set { isChecked_ToggleTheme = value; OnPropertyChanged(); } }

        public Node_ConnectionViewModel PendingConnection
        {
            get => _pendingConnection;
            set { _pendingConnection = value; OnPropertyChanged(); }
        }
        public ObservableCollection<NodeControl_NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<Node_ConnectionViewModel> Connections { get; } = new();

        public ModelRegistry NodeRegistry { get; } = new();
        public SelectionService SelectionService { get; } = new();
        public UndoRedoService UndoRedoService { get; } = new();

        private double _canvasWidth = 15000;

        public double CanvasWidth
        {
            get => _canvasWidth;
            set { SetField(ref _canvasWidth, value); }
        }
        private double _canvasHeight = 15000;

        public double CanvasHeight
        {
            get => _canvasHeight;
            set { SetField(ref _canvasHeight, value); }
        }

        private double _zoomFactor = 1.0;

        public double ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                value = Math.Clamp(value, 0.2, 3.0);

                if (SetField(ref _zoomFactor, value))
                {
                    OnPropertyChanged(nameof(CanvasWidth));
                    OnPropertyChanged(nameof(CanvasHeight));
                }
            }
        }

        private double _offsetX;

        public double OffsetX
        {
            get => _offsetX;
            set => SetField(ref _offsetX, Math.Clamp(value, -5500, 0));
        }

        private double _offsetY;

        public double OffsetY
        {
            get => _offsetY;
            set => SetField(ref _offsetY, Math.Clamp(value, -5500, 0));
        }

        public ISelectableViewModel? SelectedItem
        {
            get
            {
                var selection = SelectionService.SelectedItem;
                if (selection == null) return null;
                return NodeRegistry.GetViewModel(selection);
            }
        }

        #endregion

        #region Commands
        public ICommand ChangeTheme { get; }
        public ICommand ConvertColorClick { get; }
        public ICommand ImageImportClick { get; }
        public ICommand TestNodeClick { get; }
        public ICommand ThresholdClick { get; }
        public ICommand GaussianBlurClick { get; }
        public ICommand MedianBlurClick { get; }
        public ICommand BilateralFilterClick { get; }
        public ICommand CannyClick { get; }
        public ICommand ErodeClick { get; }
        public ICommand DilateClick { get; }
        public ICommand MorphologyExClick { get; }
        public ICommand ImageRotateClick { get; }
        public ICommand ImageResizeClick { get; }
        public ICommand ImageCropClick { get; }
        public ICommand FindContoursClick { get; }
        public ICommand FilterContoursClick { get; }

        public ICommand DrawContoursClick { get; }

        public ICommand PipelineExecuteCommand { get; }
        public ICommand ExecutePipelineCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand NewPipelineCommand { get; }
        public ICommand OpenPipelineCommand { get; }
        public ICommand SavePipelineCommand { get; }
        public ICommand SaveAsPipelineCommand { get; }

        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }

        #endregion

        public Window_MainWindowViewModel()
        {
            ChangeTheme = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (IsChecked_ToggleTheme)
                {
                    AppTheme.ChangeTheme(new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative));
                }
                else { AppTheme.ChangeTheme(new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative)); }
            });

            ConvertColorClick = new RelayCommand(
                () => Nodes.Count > 0 && Nodes.Any(n => n is ImageImport_NodeViewModel),
                () => AddNode(NodeType.ConvertColor));

            TestNodeClick = new RelayCommand(() => true, () => AddNode(NodeType.Test));
            ImageImportClick = new RelayCommand(() => true, () => AddNode(NodeType.ImageImport));
            ThresholdClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.Threshold));
            GaussianBlurClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.GaussianBlur));
            MedianBlurClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.MedianBlur));
            BilateralFilterClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.BilateralFilter));
            CannyClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.Canny));
            ErodeClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.Erode));
            DilateClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.Dilate));
            MorphologyExClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.MorphologyEx));
            ImageRotateClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.ImageRotate));
            ImageResizeClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.ImageResize));
            ImageCropClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.ImageCrop));
            FindContoursClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.FindContours));
            FilterContoursClick = new RelayCommand(() => Nodes.Count > 0, () => AddNode(NodeType.FilterContours));
            DrawContoursClick = new RelayCommand(() => Nodes.Count >= 0, () => AddNode(NodeType.DrawContours));

            UndoCommand = new RelayCommand(() => UndoRedoService.CanUndo, () => UndoRedoService.Undo());
            RedoCommand = new RelayCommand(() => UndoRedoService.CanRedo, () => UndoRedoService.Redo());
            NewPipelineCommand = new RelayCommand(() => true, () => NewPipeline());
            OpenPipelineCommand = new RelayCommand(() => true, () => OpenPipeline());
            SavePipelineCommand = new RelayCommand(() => true, () => SavePipeline());
            SaveAsPipelineCommand = new RelayCommand(() => true, () => SaveAsPipeline());

            PipelineExecuteCommand = new RelayCommand(() => Nodes.Count > 0, () => ExecutePipeline());
            ExecutePipelineCommand = PipelineExecuteCommand;

            CopyCommand = new RelayCommand(() => SelectionService.HasSelection, () => CopySelection());
            CutCommand = new RelayCommand(() => SelectionService.HasSelection, () => CutSelection());
            PasteCommand = new RelayCommand(() => ClipboardService.HasData, () => PasteSelection());

            SelectionService.SelectedItems.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(SelectedItem));
            Nodes.CollectionChanged += Model_CollectionChanged;
            Connections.CollectionChanged += Model_CollectionChanged;

            // Đánh dấu dirty khi nodes hoặc connections thay đổi
            Nodes.CollectionChanged += (s, e) => IsDirty = true;
            Connections.CollectionChanged += (s, e) => IsDirty = true;
        }

        private System.Windows.Threading.DispatcherTimer? _statusTimer;

        private void ExecutePipeline()
        {
            var executor = new VisionPipelineExecutor(Nodes, Connections);
            var result = executor.Execute();

            IsPipelineExecutedSuccess = result.Success;
            IsPipelineStatusVisible = true;
            PipelineExecutionStatusText = result.Success
                ? $"Pipeline Executed Successfully ({result.ElapsedMs} ms)"
                : $"Pipeline Execution Failed ({result.ElapsedMs} ms)";

            _statusTimer?.Stop();
            _statusTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) =>
            {
                IsPipelineStatusVisible = false;
                foreach (var node in Nodes)
                {
                    node.NodeModel.ExecutionState = NodeExecutionState.None;
                    node.NodeModel.HasError = false;
                }
                _statusTimer.Stop();
            };
            _statusTimer.Start();
        }

        private void Model_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ISelectableViewModel viewModel in e.NewItems)
                {
                    NodeRegistry.Register(viewModel);
                }
            }

            if (e.OldItems != null)
            {
                foreach (ISelectableViewModel viewModel in e.OldItems)
                {
                    NodeRegistry.Unregister(viewModel);
                }
            }
        }

        public Node_ConnectionViewModel? AddConnection(PortModel fromPort, PortModel toPort, System.Windows.Point startPoint, System.Windows.Point endPoint)
        {
            if (Connections.Any(c => c.ConnectionModel.FromPort == fromPort && c.ConnectionModel.ToPort == toPort))
                return null;

            var model = new ConnectionModel();
            model.FromPort = fromPort; model.ToPort = toPort;
            model.Start = startPoint;
            model.End = endPoint;
            var vm = new Node_ConnectionViewModel(model);
            vm.ConnectionModel.UpdateControls();
            UndoRedoService.Execute(new ConnectCommand(this, vm));
            return vm;
        }

        public void AddNode(NodeType type)
        {
            var random = new Random();
            var nodeModel = new NodeBuilder()
                .SetNodeType(type)
                .SetCoordinate(random.Next(300, 400), random.Next(50, 200))
                .Build();
            UndoRedoService.Execute(new AddNodeCommand(this, NodeFactory.Create(nodeModel)));
        }

        public void AddNode(NodeModel node)
        {
            var random = new Random();
            var nodeModel = new NodeBuilder()
                .SetNodeType(node.Type)
                .SetCoordinate(random.Next(300, 400), random.Next(50, 200))
                .Build();
            UndoRedoService.Execute(new AddNodeCommand(this, NodeFactory.Create(nodeModel)));
        }

        public void MoveNode(System.Windows.Point oldPos, System.Windows.Point newPos)
        {
            var nodes = Nodes.Where(n => n.NodeModel.IsSelected == true).ToList();
            UndoRedoService.Execute(new MoveNodeCommand(this, nodes, oldPos, newPos));
        }

        public void SelectInRect(System.Windows.Rect rect)
        {
            var items = new List<(ISelectable, System.Windows.Point)>();

            foreach (var n in Nodes)
                items.Add((n.NodeModel, new System.Windows.Point(n.NodeModel.X, n.NodeModel.Y)));

            foreach (var c in Connections)
            {
                var mid = new System.Windows.Point(
                    (c.ConnectionModel.Start.X + c.ConnectionModel.End.X) / 2,
                    (c.ConnectionModel.Start.Y + c.ConnectionModel.End.Y) / 2);
                items.Add((c.ConnectionModel, mid));
            }

            SelectionService.SelectInRect(items, rect);
        }

        public void DeleteSelection()
        {
            var selected = SelectionService.SelectedItems.ToList();

            List<NodeControl_NodeViewModel> selectedNodes = new();
            List<Node_ConnectionViewModel> selectedConnections = new();
            foreach (var item in selected)
            {
                switch (item)
                {
                    case NodeModel nodeModel:
                        selectedNodes.AddRange(Nodes.Where(n => n.NodeModel == nodeModel));
                        break;

                    case ConnectionModel connectionModel:
                        selectedConnections.AddRange(Connections.Where(c => c.ConnectionModel == connectionModel));
                        break;
                }
            }

            selectedConnections = selectedConnections.Distinct().ToList();

            var deleteNodeCmd = new DeleteNodeCommand(this, selectedNodes);
            var disconnectCmd = new DisconnectCommand(this, selectedConnections, selectedNodes);

            UndoRedoService.Execute(new CompositeCommand(deleteNodeCmd, disconnectCmd));

            SelectionService.Clear();
        }

        private void DeleteNode(NodeControl_NodeViewModel node)
        {
            var removeConnections = Connections
                .Where(c => c.ConnectionModel.FromPort.Owner == node.NodeModel || c.ConnectionModel.ToPort.Owner == node.NodeModel)
                .ToList();

            foreach (var c in removeConnections)
                Connections.Remove(c);

            Nodes.Remove(node);
        }

        public void ClearAllSelections() => SelectionService.Clear();

        #region Copy / Cut / Paste

        /// <summary>
        /// Copy các node đã chọn vào clipboard nội bộ.
        /// </summary>
        public void CopySelection()
        {
            var selectedNodes = Nodes
                .Where(n => n.NodeModel.IsSelected)
                .ToList();

            if (selectedNodes.Count == 0) return;

            ClipboardService.CopyNodes(selectedNodes, Connections);
        }

        /// <summary>
        /// Cut các node đã chọn: copy vào clipboard rồi xóa khỏi diagram.
        /// </summary>
        public void CutSelection()
        {
            var selectedNodes = Nodes
                .Where(n => n.NodeModel.IsSelected)
                .ToList();

            if (selectedNodes.Count == 0) return;

            // Copy trước
            ClipboardService.CopyNodes(selectedNodes, Connections);

            // Sau đó xóa (dùng lại logic DeleteSelection)
            DeleteSelection();
        }

        /// <summary>
        /// Paste các node từ clipboard vào diagram với offset vị trí.
        /// </summary>
        public void PasteSelection()
        {
            if (!ClipboardService.HasData) return;

            var pastedNodes = ClipboardService.CreatePastedNodes();
            if (pastedNodes.Count == 0) return;

            // Truyền bản copy của connection snapshots (không phải reference)
            var connectionSnapshots = new List<ClipboardService.ConnectionSnapshot>(ClipboardService.CopiedConnections);
            var pasteCmd = new PasteNodesCommand(this, pastedNodes, connectionSnapshots);
            UndoRedoService.Execute(pasteCmd);
        }

        #endregion

        #region New / Open / Save / Save As

        /// <summary>
        /// Tạo pipeline mới. Hỏi xác nhận nếu có thay đổi chưa lưu.
        /// </summary>
        private void NewPipeline()
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(
                    "Bạn có muốn lưu thay đổi trước khi tạo mới?",
                    "Lưu thay đổi",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    SavePipeline();
                    // Nếu user cancel dialog Save thì không tiếp tục
                    if (IsDirty) return;
                }
            }

            Nodes.Clear();
            Connections.Clear();
            SelectionService.Clear();
            IsPipelineStatusVisible = false;
            CurrentFilePath = null;
            IsDirty = false;
        }

        /// <summary>
        /// Mở file project (.mvne).
        /// </summary>
        private void OpenPipeline()
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(
                    "Bạn có muốn lưu thay đổi trước khi mở file khác?",
                    "Lưu thay đổi",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    SavePipeline();
                    if (IsDirty) return;
                }
            }

            var openDialog = new OpenFileDialog
            {
                Filter = FileFilter,
                Title = "Open Pipeline Project"
            };

            if (openDialog.ShowDialog() != true)
                return;

            try
            {
                ProjectFileService.LoadFromFile(openDialog.FileName, this);
                CurrentFilePath = openDialog.FileName;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể mở file:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lưu file. Nếu chưa có đường dẫn thì chuyển sang Save As.
        /// </summary>
        private void SavePipeline()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveAsPipeline();
                return;
            }

            try
            {
                ProjectFileService.SaveToFile(CurrentFilePath, this);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể lưu file:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lưu file với tên mới (luôn hiển thị dialog).
        /// </summary>
        private void SaveAsPipeline()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = FileFilter,
                Title = "Save Pipeline Project As",
                DefaultExt = ".mvne",
                FileName = CurrentFilePath != null
                    ? Path.GetFileName(CurrentFilePath)
                    : "Untitled.mvne"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                ProjectFileService.SaveToFile(saveDialog.FileName, this);
                CurrentFilePath = saveDialog.FileName;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể lưu file:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion
    }
}

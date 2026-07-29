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
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #endregion

        #region Properties
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

        public ICommand PipelineExecuteCommand { get; }
        public ICommand ExecutePipelineCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand NewPipelineCommand { get; }

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

            UndoCommand = new RelayCommand(() => UndoRedoService.CanUndo, () => UndoRedoService.Undo());
            RedoCommand = new RelayCommand(() => UndoRedoService.CanRedo, () => UndoRedoService.Redo());
            NewPipelineCommand = new RelayCommand(() => true, () =>
            {
                Nodes.Clear();
                Connections.Clear();
                SelectionService.Clear();
                IsPipelineStatusVisible = false;
            });

            PipelineExecuteCommand = new RelayCommand(() => Nodes.Count > 0, () => ExecutePipeline());
            ExecutePipelineCommand = PipelineExecuteCommand;

            SelectionService.SelectedItems.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(SelectedItem));
            Nodes.CollectionChanged += Model_CollectionChanged;
            Connections.CollectionChanged += Model_CollectionChanged;
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
    }
}

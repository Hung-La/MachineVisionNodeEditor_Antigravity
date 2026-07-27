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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static OpenCvSharp.ML.DTrees;

namespace MachineVisionNodeEditor.ViewModels.WindowViewModels
{
    public class Window_MainWindowViewModel : BaseViewModel
    {
        #region Fields
        private bool isChecked_ToggleTheme;
        private Node_ConnectionViewModel _pendingConnection;
        private bool _isDraggingConnection;

        #endregion

        #region Properties
        public bool IsChecked_ToggleTheme { get => isChecked_ToggleTheme; set { isChecked_ToggleTheme = value; OnPropertyChanged(); } }

        public Node_ConnectionViewModel PendingConnection
        {
            get => _pendingConnection;
            set { _pendingConnection = value; OnPropertyChanged(); }
        }
        public ObservableCollection<NodeControl_NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<Node_ConnectionViewModel> Connections { get; } = new();

        public ModelRegistry NodeRegistry { get; } = new();

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
                var selectedItem = SelectionService.SelectedItem;
                if (selectedItem == null) return null;
                else
                {
                    
                    switch (selectedItem)
                    {
                        //case ImageImport_NodeModel imageImport_NodeModel:
                        //    {
                        //        return Nodes.FirstOrDefault(n => n.NodeModel == imageImport_NodeModel);
                        //    }
                        //case ConvertColor_NodeModel convertColor_NodeModel:
                        //    {
                        //        return Nodes.FirstOrDefault(n => n.NodeModel == convertColor_NodeModel);
                        //    }
                        //case Test_NodeModel test_NodeModel:
                        //    {
                        //        return Nodes.FirstOrDefault(n => n.NodeModel == test_NodeModel);
                        //    }
                        case NodeModel nodeModel:
                            {
                                return Nodes.FirstOrDefault(n => n.NodeModel == nodeModel);
                            }
                        case ConnectionModel connectionModel:
                            {
                                return Connections.FirstOrDefault(c => c.ConnectionModel == connectionModel);
                            }
                        default: return null;
                    }
                }

            }
        }

        #endregion

        #region Services
        public SelectionService SelectionService { get; } = new SelectionService();
        public UndoRedoService UndoRedoService { get; } = new UndoRedoService();
        #endregion

        #region Commands
        public ICommand ChangeTheme { get; }
        public ICommand ConvertColorClick { get; }
        public ICommand ImageImportClick { get; }
        public ICommand TestNodeClick { get; }

        public ICommand RunPipelineCommand { get; }

        #endregion

        public Window_MainWindowViewModel()
        {
            ChangeTheme = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                if (IsChecked_ToggleTheme)
                {
                    AppTheme.
                    ChangeTheme(new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative));
                }
                else { AppTheme.ChangeTheme(new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative)); }
            });

            ConvertColorClick = new RelayCommand(
                () =>
                {
                    if (Nodes.Count == 0)
                    {
                        return false;
                    }
                    else if (Nodes.Count > 0)
                    {
                        if (Nodes.Any(n => n is ImageImport_NodeViewModel))
                        {
                            return true;
                        }
                    }

                    return false;

                },
                () =>
                {
                    AddNode(NodeType.ConvertColor);
                });

            TestNodeClick = new RelayCommand(
                () =>
                {
                    return true;
                },
                () =>
                {
                    AddNode(NodeType.Test);
                });

            ImageImportClick = new RelayCommand(
                () =>
                {
                    return true;
                },
                () =>
                {
                    AddNode(NodeType.ImageImport);
                });

            RunPipelineCommand = new RelayCommand(
                () =>
                {
                    // Điều kiện để nút Run sáng lên: Phải có ít nhất 1 Node trên Canvas
                    return Nodes.Count > 0;
                },
                () =>
                {
                    // TODO: Chỗ này chúng ta sẽ khởi tạo và gọi class VisionPipelineExecutor sau.
                    // Tạm thời hiển thị thông báo để test nút nhấn đã binding thành công chưa.
                    MessageBox.Show("Bắt đầu kích hoạt luồng Pipeline xử lý ảnh...", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                });

            // Khi selection thay đổi → notify SelectedViewModel để Properties panel cập nhật
            SelectionService.SelectedItems.CollectionChanged += (sender, e)
                => OnPropertyChanged(nameof(SelectedItem));


            // Khi Nodes và Connections thay đổi thì sẽ cập nhật NodeModel
            Nodes.CollectionChanged += Model_CollectionChanged;
            Connections.CollectionChanged += Model_CollectionChanged;
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

        public Node_ConnectionViewModel? AddConnection(PortModel fromPort, PortModel toPort, Point startPoint, Point endPoint)
        {
            // Không cho nối 2 lần cùng cặp port
            if (Connections.Any(c => c.ConnectionModel.FromPort == fromPort && c.ConnectionModel.ToPort == toPort))
                return null;

            var model = new ConnectionModel();
            model.FromPort = fromPort; model.ToPort = toPort;
            model.Start = startPoint;
            model.End = endPoint;
            var vm = new Node_ConnectionViewModel(model);
            vm.ConnectionModel.UpdateControls();
            //Connections.Add(vm);
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


        public void MoveNode(Point oldPos, Point newPos)
        {
            var nodes = Nodes.Where(n => n.NodeModel.IsSelected == true).ToList();
            UndoRedoService.Execute(new MoveNodeCommand(this, nodes, oldPos, newPos));

        }

        public void SelectInRect(Rect rect)
        {
            var items = new List<(ISelectable, Point)>();

            foreach (var n in Nodes)
                items.Add((n.NodeModel, new Point(n.NodeModel.X, n.NodeModel.Y)));

            foreach (var c in Connections)
            {
                var mid = new Point(
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
                        selectedNodes.AddRange(
                            Nodes.Where(n => n.NodeModel == nodeModel));
                        break;

                    case ConnectionModel connectionModel:
                        selectedConnections.AddRange(
                            Connections.Where(c => c.ConnectionModel == connectionModel));
                        break;
                }
            }

            selectedConnections = selectedConnections
                                    .Distinct()
                                    .ToList();


            //UndoRedoService.Execute(new DeleteNodeCommand(this, selectedNodes));
            //UndoRedoService.Execute(new DisconnectCommand(this, selectedConnections, selectedNodes));

            // ✅ Gộp 2 command thành 1, thực thi và push 1 lần duy nhất
            var deleteNodeCmd = new DeleteNodeCommand(this, selectedNodes);
            var disconnectCmd = new DisconnectCommand(this, selectedConnections, selectedNodes);

            UndoRedoService.Execute(new CompositeCommand(deleteNodeCmd, disconnectCmd));

            SelectionService.Clear();
        }

        private void DeleteNode(NodeControl_NodeViewModel node)
        {
            var removeConnections =
                Connections
                .Where(c => c.ConnectionModel.FromPort.Owner == node.NodeModel || c.ConnectionModel.ToPort.Owner == node.NodeModel)
                .ToList();

            foreach (var c in removeConnections)
                Connections.Remove(c);

            Nodes.Remove(node);
        }

        public void ClearAllSelections() => SelectionService.Clear();
    }
}




using MachineVisionAlgorithm.ImageInteraction;
using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class ImageImport_NodeViewModel : NodeControl_NodeViewModel
        <ImageImport_NodeModel, 
        ImageImport_NodePropertyModel,
        ImageImport_NodeOperationModel>
    {
        #region Fields


        #endregion

        #region Properties
        //private ImageImport_NodeModel _imageImport_NodeModel;

        //public ImageImport_NodeModel ImageImport_NodeModel { get => _imageImport_NodeModel; set { _imageImport_NodeModel = value; OnPropertyChanged(); } }

        //private NodeModel nodeModel;

        //public NodeModel NodeModel { get => nodeModel; set { nodeModel = value; OnPropertyChanged(); } }

        #endregion

        #region Commands
        public ICommand BrowseCommand { get; private set; }
        public override ICommand ShowImageCommand { get; protected set; }

        #endregion

        public ImageImport_NodeViewModel() : base(new ImageImport_NodeModel())
        {
            Initialize();
        }

        public ImageImport_NodeViewModel(ImageImport_NodeModel nodeModel) : base(nodeModel)
        {
            Initialize();
        }

        public ImageImport_NodeViewModel(NodeModel nodeModel) : base(nodeModel is ImageImport_NodeModel im ? im : new ImageImport_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.ImageImport
        })
        {
            Initialize();
        }

        private void Initialize()
        {
            NodeModel.Title = "Image Import";

            BrowseCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                string filePath = @"C:\";
                DirectoryInfo Folder = new DirectoryInfo(filePath);
                OpenFileDialog openFileDialog = new OpenFileDialog();

                try
                {
                    openFileDialog.InitialDirectory = Folder.ToString();
                    openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*";
                    openFileDialog.ShowDialog();

                    string ChoosenFile = openFileDialog.FileName;

                    if (!string.IsNullOrEmpty(ChoosenFile))
                    {
                        NodePropertyModel.FilePath = ChoosenFile;
                        NodePropertyModel.Name = Path.GetFileNameWithoutExtension(ChoosenFile);
                        NodePropertyModel.Description = Path.GetExtension(ChoosenFile);
                    }

                    if (File.Exists(NodePropertyModel.FilePath))
                    {
                        var mode = NodePropertyModel.SelectedMode != null ? (ImreadModes)NodePropertyModel.SelectedMode : ImreadModes.Color;
                        var img = ImageImport.ReadImage(NodePropertyModel.FilePath, mode);
                        NodePropertyModel.Context.InputImage = img;
                        NodePropertyModel.Context.OutputImage = img;
                        if (img != null)
                        {
                            NodePropertyModel.Context.OutputImages = new List<Mat> { img };
                            NodePropertyModel.Width = img.Width;
                            NodePropertyModel.Height = img.Height;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });

            ShowImageCommand = new RelayCommand(() => 
                {
                    return NodePropertyModel != null && File.Exists(NodePropertyModel.FilePath);
                },
                    () =>
                {
                    if (NodePropertyModel == null) return;

                    if (NodePropertyModel.Context.OutputImage == null && File.Exists(NodePropertyModel.FilePath))
                    {
                        var mode = NodePropertyModel.SelectedMode != null ? (ImreadModes)NodePropertyModel.SelectedMode : ImreadModes.Color;
                        var img = ImageImport.ReadImage(NodePropertyModel.FilePath, mode);
                        NodePropertyModel.Context.InputImage = img;
                        NodePropertyModel.Context.OutputImage = img;
                        if (img != null)
                        {
                            NodePropertyModel.Context.OutputImages = new List<Mat> { img };
                            NodePropertyModel.Width = img.Width;
                            NodePropertyModel.Height = img.Height;
                        }
                    }

                    ShowNodeImages();
                });

            EnsureInitialPorts();
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override ImageImport_NodeOperationModel CreateOperationModel()
        {
            return new ImageImport_NodeOperationModel();
        }
    }
}

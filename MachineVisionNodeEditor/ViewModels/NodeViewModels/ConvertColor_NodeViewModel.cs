using MachineVisionNodeEditor.Commands;
using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.Windows;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using OpenCvSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class ConvertColor_NodeViewModel : NodeControl_NodeViewModel
        <ConvertColor_NodeModel, 
        ConvertColor_NodePropertyModel,
        ConvertColor_NodeOperationModel>
    {
        #region Commands
        public ICommand ShowImageCommand { get; private set; }

        #endregion

        public ConvertColor_NodeViewModel() : base()
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ConvertColor_NodeViewModel(ConvertColor_NodeModel model) : base(model)
        {
            Initialize();
            EnsureInitialPorts();
        }

        public ConvertColor_NodeViewModel(NodeModel nodeModel) : base(nodeModel is ConvertColor_NodeModel vm ? vm : new ConvertColor_NodeModel
        {
            X = nodeModel.X,
            Y = nodeModel.Y,
            Type = NodeType.ConvertColor
        })
        {
            Initialize();
            EnsureInitialPorts();
        }

        private void Initialize()
        {
            NodeModel.Title = "Convert Color";

            ShowImageCommand = new RelayCommand(() =>
            {
                if (NodePropertyModel == null || this.NodeModel.InputPorts[0].PortModel.Connections.Count == 0)
                {
                    return false;
                }
                foreach (var item in this.NodeModel.InputPorts[0].PortModel.Connections)
                {
                    if (ImageHelper.GetImageFromPreviousNode(item) != null)
                    {
                        if(NodePropertyModel.SelectedCode != null)
                        {
                            return true;
                        }

                    }

                }
                return false;

            },
            () =>
            {
                if (NodePropertyModel == null) return;
                List<Mat> images = new();
                foreach (var item in this.NodeModel.InputPorts[0].PortModel.Connections)
                {
                    var image = ImageHelper.GetImageFromPreviousNode(item);
                    if (image != null)
                    {
                        images.Add(image);
                    }
                   
                }

                foreach(var item in images)
                {
                    NodePropertyModel.InputImage = item;
                    OperationModel.Execute(NodePropertyModel);
                    if (NodePropertyModel.OutputImage != null)
                    {
                        var convertColorWindow = new NodeWindow();
                        convertColorWindow.DataContext = this;
                        convertColorWindow.Show();
                    }
                }


            });
        }

        private void EnsureInitialPorts()
        {
            if (NodeModel.InputPorts.Count == 0) NodeModel.AddPort(PortType.Input);
            if (NodeModel.OutputPorts.Count == 0) NodeModel.AddPort(PortType.Output);
        }

        protected override ConvertColor_NodeOperationModel CreateOperationModel()
        {
            return new ConvertColor_NodeOperationModel();
        }
    }
}

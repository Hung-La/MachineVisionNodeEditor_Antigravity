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
        public override ICommand ShowImageCommand { get; protected set; }

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

            ShowImageCommand = new RelayCommand(
                () => NodePropertyModel?.Context.OutputImage != null && !NodePropertyModel.Context.OutputImage.IsDisposed && !NodePropertyModel.Context.OutputImage.Empty(),
                () => ShowNodeImages());
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

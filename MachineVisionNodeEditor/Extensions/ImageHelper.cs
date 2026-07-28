using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Windows;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Extensions
{
    public class ImageHelper
    {
        public static Mat? GetImageFromPreviousNode(ConnectionModel connection)
        {
            var registry = MainWindow.Instance.Window_MainWindowViewModel.NodeRegistry;
            var viewModel = (NodeControl_NodeViewModel)registry.GetViewModel(connection.FromPort.Owner);
            var mat = viewModel.NodePropertyModel.OutputImage;
            return mat != null ? mat : null;
        }
    }
}

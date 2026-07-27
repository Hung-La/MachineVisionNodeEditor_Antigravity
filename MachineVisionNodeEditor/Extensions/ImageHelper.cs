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
            if (viewModel.NodePropertyModel.DestinationImage != null)
            {
                return viewModel.NodePropertyModel.DestinationImage;
            }

            return null;
        }
    }
}

using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace MachineVisionNodeEditor.Views.NodeProperties
{
    public class NodePropertyControl : UserControl
    {
        public NodePropertyControl()
        {
            DataContextChanged += NodePropertyControl_DataContextChanged;
            Loaded += NodePropertyControl_Loaded;
        }

        private void NodePropertyControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateViewReference(DataContext);
        }

        private void NodePropertyControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            UpdateViewReference(e.NewValue);
        }

        private void UpdateViewReference(object? dataContext)
        {
            if (dataContext is INodeViewModel viewModel)
            {
                viewModel.NodePropertyModel.View = this;
            }
            else if (dataContext is Node_ConnectionViewModel connectionViewModel)
            {
                connectionViewModel.ConnectionPropertyModel.View = this;
            }
        }
    }
}

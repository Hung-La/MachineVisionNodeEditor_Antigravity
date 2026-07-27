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
            Loaded += NodePropertyControl_Loaded;
        }

        protected void NodePropertyControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataContext = MainWindow.Instance.Window_MainWindowViewModel.SelectedItem;
            if (DataContext != null)
            {
                if (DataContext is INodeViewModel viewModel)
                {
                    viewModel.NodePropertyModel.View = this;
                }
                else if (DataContext is Node_ConnectionViewModel connectionViewModel)
                {
                    connectionViewModel.ConnectionPropertyModel.View = this;
                }
            }
        }


    }
}

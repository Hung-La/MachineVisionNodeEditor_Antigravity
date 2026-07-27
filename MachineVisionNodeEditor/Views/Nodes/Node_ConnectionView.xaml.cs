using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MachineVisionNodeEditor.Views.Nodes
{
    /// <summary>
    /// Interaction logic for Node_ConnectionView.xaml
    /// </summary>
    public partial class Node_ConnectionView : UserControl
    {
        public Node_ConnectionView()
        {
            InitializeComponent();
        }

        private void Connector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Path path)
                return;

            if (path.DataContext is Node_ConnectionViewModel vm)
            {
                if (vm.ConnectionModel is ISelectable selectable)
                {
                    bool ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                    MainWindow.Instance.Window_MainWindowViewModel
                        .SelectionService
                        .Select(selectable, ctrl);

                    e.Handled = true;
                }
            }
        }

    }
}

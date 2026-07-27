using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
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

namespace MachineVisionNodeEditor.Views.NodeProperties
{
    /// <summary>
    /// Interaction logic for ImageImportPropertyView.xaml
    /// </summary>
    public partial class ImageImport_PropertyView : NodePropertyControl
    {
        public ImageImport_PropertyView()
        {
            InitializeComponent();
            //Loaded += ImageImportPropertyView_Loaded;
        }

        private void ImageImportPropertyView_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = MainWindow.Instance.Window_MainWindowViewModel.SelectedItem;
            if (DataContext != null)
            {
                if (DataContext is INodeViewModel viewModel)
                {
                    viewModel.NodePropertyModel.View = this;
                }
            }
        }
    }
}

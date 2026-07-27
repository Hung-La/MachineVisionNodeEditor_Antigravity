using MachineVisionNodeEditor.ViewModels.UserControlViewModels;
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

namespace MachineVisionNodeEditor.Views.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl_ControlBar.xaml
    /// </summary>
    public partial class UserControl_ControlBar : UserControl
    {
        public UserControl_ControlBarViewModel UserControl_ControlBarViewModel { get; set; }
        public UserControl_ControlBar()
        {
            InitializeComponent();
            this.DataContext = UserControl_ControlBarViewModel = new UserControl_ControlBarViewModel();
        }
    }
}

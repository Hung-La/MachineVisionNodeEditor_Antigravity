using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for ConvertColor_NodeView.xaml
    /// </summary>
    public partial class ConvertColor_NodeView : NodeControl
    {
        public ConvertColor_NodeView()
        {
            InitializeComponent();
            Loaded += NodeControl_Loaded;
        }

    }
}

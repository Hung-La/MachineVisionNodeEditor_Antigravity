using MachineVisionNodeEditor.Views.Nodes;
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
using System.Windows.Shapes;

namespace MachineVisionNodeEditor.Views.Windows.NodeWindows
{
    public class NamedColorBrush
    {
        public string Name { get; set; } = string.Empty;
        public Brush Brush { get; set; } = Brushes.Transparent;
    }

    /// <summary>
    /// Interaction logic for NodeWindow.xaml
    /// </summary>
    public partial class NodeWindow : NodeWindowControl
    {
        public NodeWindow()
        {
            InitializeComponent();

            var colors = new List<NamedColorBrush>
            {
                new NamedColorBrush { Name = "Dim Gray", Brush = Brushes.DimGray },
                new NamedColorBrush { Name = "Red", Brush = Brushes.Red },
                new NamedColorBrush { Name = "Green", Brush = Brushes.LimeGreen },
                new NamedColorBrush { Name = "Blue", Brush = Brushes.DeepSkyBlue },
                new NamedColorBrush { Name = "Yellow", Brush = Brushes.Yellow },
                new NamedColorBrush { Name = "Cyan", Brush = Brushes.Cyan },
                new NamedColorBrush { Name = "Magenta", Brush = Brushes.Magenta },
                new NamedColorBrush { Name = "White", Brush = Brushes.White },
                new NamedColorBrush { Name = "Black", Brush = Brushes.Black },
                new NamedColorBrush { Name = "Orange", Brush = Brushes.Orange }
            };

            GridColorCombo.ItemsSource = colors;
            GridColorCombo.SelectedIndex = 0;
        }
    }
}

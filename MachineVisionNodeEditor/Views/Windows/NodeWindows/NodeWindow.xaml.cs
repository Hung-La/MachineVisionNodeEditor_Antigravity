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
        public int ImageIndex { get; set; } = 0;
        private ViewModels.NodeViewModels.NodeControl_NodeViewModel? _vm;

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

            DataContextChanged += NodeWindow_DataContextChanged;
            Closed += NodeWindow_Closed;
        }

        public NodeWindow(object dataContext, OpenCvSharp.Mat image, int imageIndex = 0) : this()
        {
            ImageIndex = imageIndex;
            DataContext = dataContext;
            UpdateImage();
        }

        private void NodeWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ViewModels.NodeViewModels.NodeControl_NodeViewModel oldVm && oldVm.NodePropertyModel?.Context != null)
            {
                oldVm.NodePropertyModel.Context.PropertyChanged -= Context_PropertyChanged;
            }

            _vm = DataContext as ViewModels.NodeViewModels.NodeControl_NodeViewModel;
            if (_vm?.NodePropertyModel?.Context != null)
            {
                _vm.NodePropertyModel.Context.PropertyChanged += Context_PropertyChanged;
            }

            UpdateImage();
        }

        private void Context_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Models.NodeContextModels.NodeContext.OutputImages) ||
                e.PropertyName == nameof(Models.NodeContextModels.NodeContext.OutputImage) ||
                e.PropertyName == "Outputs")
            {
                if (Dispatcher.CheckAccess())
                {
                    UpdateImage();
                }
                else
                {
                    Dispatcher.Invoke(UpdateImage);
                }
            }
        }

        private void UpdateImage()
        {
            if (_vm?.NodePropertyModel?.Context == null) return;

            var context = _vm.NodePropertyModel.Context;
            var outputs = context.OutputImages;

            if (outputs != null && ImageIndex >= 0 && ImageIndex < outputs.Count)
            {
                var img = outputs[ImageIndex];
                if (img != null && !img.IsDisposed && !img.Empty())
                {
                    ImageViewer.ViewImage = img;
                    return;
                }
            }

            if (context.OutputImage != null && !context.OutputImage.IsDisposed && !context.OutputImage.Empty())
            {
                ImageViewer.ViewImage = context.OutputImage;
            }
        }

        private void NodeWindow_Closed(object? sender, EventArgs e)
        {
            DataContextChanged -= NodeWindow_DataContextChanged;
            Closed -= NodeWindow_Closed;
            if (_vm?.NodePropertyModel?.Context != null)
            {
                _vm.NodePropertyModel.Context.PropertyChanged -= Context_PropertyChanged;
            }
        }
    }
}

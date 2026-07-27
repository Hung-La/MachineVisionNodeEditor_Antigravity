using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Registries;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MachineVisionNodeEditor.Converters
{
    public class NodeViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeModel nodeModel)
            {
                var viewModel = nodeModel.View.DataContext;

                if (viewModel != null)
                {
                    if (viewModel is NodeControl_NodeViewModel nodeViewModel)
                    {
                        try
                        {
                            return nodeViewModel.NodePropertyModel.Name;
                            //return "TestNode";
                        }
                        catch (NullReferenceException ex) { }
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

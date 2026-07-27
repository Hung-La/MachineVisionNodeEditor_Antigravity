using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.Views.NodeProperties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ConnectionPropertyModel : BaseViewModel
    {

        private double _length;
        public double Length
        {
            get => _length;
            set { SetField(ref _length, value); }
        }

        public NodePropertyControl View
        {
            get; set;
        }
    }
}

using MachineVisionNodeEditor.Models.NodeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Node_PortViewModel : BaseViewModel
    {
        private PortModel _portModel = new PortModel();

        public PortModel PortModel { get => _portModel; set { _portModel = value; OnPropertyChanged(nameof(_portModel)); } } 
    }
}

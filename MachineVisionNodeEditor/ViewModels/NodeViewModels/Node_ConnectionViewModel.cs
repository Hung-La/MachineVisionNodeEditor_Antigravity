using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public class Node_ConnectionViewModel : BaseViewModel, ISelectableViewModel
    {
        private ConnectionModel _connectionModel;

        public ConnectionModel ConnectionModel
        {
            get => _connectionModel;
            set { _connectionModel = value; OnPropertyChanged(nameof(_connectionModel)); }

        }
        public ISelectable Model => ConnectionModel;

        private ConnectionPropertyModel _connectionPropertyModel;

        public ConnectionPropertyModel ConnectionPropertyModel 
        { 
            get => _connectionPropertyModel; 
            set { SetField(ref _connectionPropertyModel, value); }
        }


        public Node_ConnectionViewModel(ConnectionModel connectionModel)
        {
            ConnectionModel = connectionModel;
            Initialize();
        }

        public Node_ConnectionViewModel() 
        {
            Initialize();
        }

        private void Initialize()
        {
            ConnectionPropertyModel = new();
        }
    }
}

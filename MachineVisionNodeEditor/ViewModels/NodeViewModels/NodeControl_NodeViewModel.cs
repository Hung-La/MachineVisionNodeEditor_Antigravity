using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodeOperationModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public abstract class NodeControl_NodeViewModel : BaseViewModel, INodeViewModel, ISelectableViewModel
    {
        private NodeModel _nodeModel;

        public NodeModel NodeModel { get => _nodeModel; set { _nodeModel = value; OnPropertyChanged(nameof(_nodeModel)); } }

        private NodePropertyModel _nodePropertyModel;

        public NodePropertyModel NodePropertyModel
        {
            get { return _nodePropertyModel; }
            protected set { SetField(ref _nodePropertyModel, value); }
        }

        public UserControl PropertyView { get; set; }

        public ISelectable Model { get {  return _nodeModel; }  }

        protected NodeControl_NodeViewModel(NodeModel nodeModel )
        {
            NodeModel = nodeModel;
        }

        protected NodeControl_NodeViewModel() { }

        protected NodeControl_NodeViewModel(
        NodeModel nodeModel,
        NodePropertyModel propertyModel)
        {
            NodeModel = nodeModel;
            NodePropertyModel = propertyModel;
        }
    }

    public abstract class NodeControl_NodeViewModel<TModel, TPropertyModel, TOperationModel> : NodeControl_NodeViewModel
        where TModel : NodeModel, new()
        where TPropertyModel : NodePropertyModel, new()
        where TOperationModel : INodeOperation, new()
    {
        /// <summary>
        /// Trả về NodeModel đã được cast sang TModel.
        /// Dùng để binding trong XAML thay vì phải cast thủ công.
        /// VD: {Binding TypedModel.FilePath}
        /// </summary>
        public TModel TypedModel => (TModel)base.NodeModel;

        public new TPropertyModel NodePropertyModel
        {
            get => (TPropertyModel)base.NodePropertyModel;
            set => base.NodePropertyModel = value;
        }

        public TOperationModel OperationModel { get; private set; }

        protected NodeControl_NodeViewModel(TModel model) : base(model) 
        {
            Initialize();
        }

        protected NodeControl_NodeViewModel() : base(new TModel()) 
        {
            Initialize();
        }

        private void Initialize()
        {
            NodePropertyModel = new TPropertyModel();
            OperationModel = CreateOperationModel();
        }

        protected abstract TOperationModel CreateOperationModel();

        //protected TOperationModel CreateOperationModel()
        //{
        //    return new TOperationModel();
        //}

    }
}

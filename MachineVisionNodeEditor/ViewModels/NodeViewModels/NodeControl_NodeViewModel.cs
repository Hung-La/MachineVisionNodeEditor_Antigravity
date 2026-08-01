using MachineVisionNodeEditor.Commands;
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
using System.Windows.Input;
using System.Windows.Navigation;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    public abstract class NodeControl_NodeViewModel : BaseViewModel, INodeViewModel, ISelectableViewModel, IImageShowable
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

        public ISelectable Model { get { return _nodeModel; } }

        public abstract ICommand ShowImageCommand
        {
            get; protected set;
        }

        public void ShowNodeImages()
        {
            if (NodePropertyModel == null) return;

            var outputs = NodePropertyModel.Context.OutputImages;
            if (outputs != null && outputs.Count > 1)
            {
                for (int i = 0; i < outputs.Count; i++)
                {
                    var img = outputs[i];
                    if (img != null && !img.IsDisposed && !img.Empty())
                    {
                        var win = new Views.Windows.NodeWindows.NodeWindow(this, img);
                        win.Title = $"{NodeModel.Title} - Image {i + 1}";
                        win.Show();
                    }
                }
            }
            else if (NodePropertyModel.Context.OutputImage != null &&
                     !NodePropertyModel.Context.OutputImage.IsDisposed &&
                     !NodePropertyModel.Context.OutputImage.Empty())
            {
                var win = new Views.Windows.NodeWindows.NodeWindow { DataContext = this };
                win.Show();
            }
        }

        protected NodeControl_NodeViewModel(NodeModel nodeModel)
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

            NodePropertyModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != nameof(NodePropertyModel.Name) &&
                    e.PropertyName != nameof(NodePropertyModel.Description) &&
                    e.PropertyName != nameof(NodePropertyModel.Width) &&
                    e.PropertyName != nameof(NodePropertyModel.Height))
                {
                    try
                    {
                        var inputImgs = NodePropertyModel.Context.InputImages;
                        if (this is ImageImport_NodeViewModel)
                        {
                            if (OperationModel is INodeOperation<TPropertyModel> genericOp)
                            {
                                genericOp.Execute(NodePropertyModel);
                            }
                        }
                        else if (inputImgs != null && inputImgs.Count > 1)
                        {
                            var newOutputs = new List<OpenCvSharp.Mat>();
                            foreach (var inputImg in inputImgs)
                            {
                                if (inputImg != null && !inputImg.IsDisposed && !inputImg.Empty())
                                {
                                    NodePropertyModel.Context.InputImage = inputImg;
                                    if (OperationModel is INodeOperation<TPropertyModel> genericOp)
                                    {
                                        genericOp.Execute(NodePropertyModel);
                                        if (NodePropertyModel.Context.OutputImage != null)
                                            newOutputs.Add(NodePropertyModel.Context.OutputImage);
                                    }
                                }
                            }
                            NodePropertyModel.Context.OutputImages = newOutputs;
                            if (newOutputs.Count > 0)
                                NodePropertyModel.Context.OutputImage = newOutputs[0];
                        }
                        else
                        {
                            var inputImg = NodePropertyModel.Context.InputImage;
                            if (inputImg != null && !inputImg.IsDisposed && !inputImg.Empty())
                            {
                                if (OperationModel is INodeOperation<TPropertyModel> genericOp)
                                {
                                    genericOp.Execute(NodePropertyModel);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"Error auto-executing node: {ex.Message}");
                    }
                }
            };
        }

        protected abstract TOperationModel CreateOperationModel();

        //protected TOperationModel CreateOperationModel()
        //{
        //    return new TOperationModel();
        //}

    }
}

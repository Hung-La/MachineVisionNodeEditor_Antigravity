using MachineVisionNodeEditor.Models.NodeModels;
using System;

namespace MachineVisionNodeEditor.ViewModels.NodeViewModels
{
    [Obsolete("Use DrawOnImage_NodeViewModel instead.")]
    public class DrawContours_NodeViewModel : DrawOnImage_NodeViewModel
    {
        public DrawContours_NodeViewModel() : base() { }
        public DrawContours_NodeViewModel(DrawOnImage_NodeModel model) : base(model) { }
        public DrawContours_NodeViewModel(NodeModel nodeModel) : base(nodeModel) { }
    }
}

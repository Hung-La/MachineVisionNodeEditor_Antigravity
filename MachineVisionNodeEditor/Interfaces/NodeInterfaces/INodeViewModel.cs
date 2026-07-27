using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.Views.NodeProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Interfaces.NodeInterfaces
{
    public interface INodeViewModel
    {
        NodeModel NodeModel { get; }

        NodePropertyModel NodePropertyModel { get; }
    }
}

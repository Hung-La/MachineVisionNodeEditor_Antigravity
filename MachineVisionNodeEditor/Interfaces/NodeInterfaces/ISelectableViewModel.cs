using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Interfaces.NodeInterfaces
{
    public interface ISelectableViewModel
    {
        ISelectable Model { get; }

    }
}

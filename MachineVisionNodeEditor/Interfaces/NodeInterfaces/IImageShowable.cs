using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MachineVisionNodeEditor.Interfaces.NodeInterfaces
{
    public interface IImageShowable
    {
        ICommand ShowImageCommand { get; }
    }
}

using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    public class AddNodeCommand : IUndoableCommand
    {
        private readonly Window_MainWindowViewModel _diagram;
        private readonly NodeControl_NodeViewModel _node;

        public AddNodeCommand(Window_MainWindowViewModel diagram,NodeControl_NodeViewModel node)
        {
            _diagram = diagram;
            _node = node;
        }

        public void Execute()
        {
            _diagram.Nodes.Add(_node);
        }

        public void Undo()
        {
            _diagram.Nodes.Remove(_node);
        }
    }
}

using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenCvSharp.ML.DTrees;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    public class DeleteNodeCommand : IUndoableCommand
    {
        private readonly Window_MainWindowViewModel _diagram;

        private readonly List<NodeControl_NodeViewModel> _nodes;


        public DeleteNodeCommand(Window_MainWindowViewModel diagram, List<NodeControl_NodeViewModel> nodes)
        {
            _diagram = diagram;

            _nodes = nodes;
        }

        public void Execute()
        {

            foreach (var node in _nodes)
            {
                _diagram.Nodes.Remove(node);
            }

        }

        public void Undo()
        {
            foreach (var node in _nodes)
            {
                _diagram.Nodes.Add(node);
            }

        }
    }
}

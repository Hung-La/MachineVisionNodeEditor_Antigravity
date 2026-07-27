using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    public class MoveNodeCommand : IUndoableCommand
    {
        private readonly List<NodeControl_NodeViewModel> _nodes;

        private readonly List<Node_ConnectionViewModel> _connections;

        private static readonly Queue<NodeControl_NodeViewModel> _nodeQueue = new();

        private readonly Point _oldPos;

        private readonly Point _newPos;

        private readonly Vector _delta;

        public MoveNodeCommand(Window_MainWindowViewModel diagram, List<NodeControl_NodeViewModel> nodes, Point oldPos, Point newPos)
        {
            _nodes = nodes;

            if (nodes.Count() > 0)
            {
                _nodeQueue.Clear();
                _nodes.ForEach(n => _nodeQueue.Enqueue(n));
            }

            _oldPos = oldPos;
            _newPos = newPos;

            _delta = newPos - oldPos;

            _connections = diagram.Connections
                .Where(c =>
                    nodes.Any(n => n.NodeModel == c.ConnectionModel.FromPort.Owner) ||
                    nodes.Any(n => n.NodeModel == c.ConnectionModel.ToPort.Owner))
                .ToList();
        }

        public void Execute()
        {
            Apply(_newPos);
        }

        public void Undo()
        {
            Apply(_oldPos);
        }

        private void Apply(Point position)
        {
            Vector move;

            if (_nodeQueue.Count == 0)
            {
                move = -_delta;
            }

            while (_nodeQueue.Count > 0)
            {
                
                if (move.X >= 1 || move.Y >= 1)
                {
                    move = position - new Point(_nodeQueue.Peek().NodeModel.X, _nodeQueue.Peek().NodeModel.Y);
                    break;
                }
                _nodeQueue.Dequeue();
            }


            _nodes.ForEach(n => n.NodeModel.X += move.X);
            _nodes.ForEach(n => n.NodeModel.Y += move.Y);

            _nodes.ForEach(n => n.NodeModel.InputPorts.ToList().ForEach(p => p.PortModel.Position += move));

            _nodes.ForEach(n => n.NodeModel.OutputPorts.ToList().ForEach(p => p.PortModel.Position += move));

            foreach (var connection in _connections)
            {

                if (_nodes.Any(n => n.NodeModel == connection.ConnectionModel.FromPort.Owner))
                {
                    connection.ConnectionModel.Start =
                        connection.ConnectionModel.FromPort.Position;
                }

                if (_nodes.Any(n => n.NodeModel == connection.ConnectionModel.ToPort.Owner))
                {
                    connection.ConnectionModel.End =
                        connection.ConnectionModel.ToPort.Position;
                }

                connection.ConnectionModel.UpdateControls();
            }
        }
    }
}



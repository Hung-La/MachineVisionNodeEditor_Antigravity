using MachineVisionNodeEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    public class CompositeCommand : IUndoableCommand
    {
        private readonly List<IUndoableCommand> _commands;

        public CompositeCommand(params IUndoableCommand[] commands)
        {
            _commands = new List<IUndoableCommand>(commands);
        }
        public void Execute()
        {
            // Thực thi theo thứ tự thường
            foreach (var cmd in _commands)
                cmd.Execute();
        }

        public void Undo()
        {
            // Undo theo thứ tự NGƯỢC LẠI (quan trọng!)
            for (int i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo();
        }
    }
}

using MachineVisionNodeEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Services
{
    public class UndoRedoService
    {
        private readonly Stack<IUndoableCommand> _undo = new();
        private readonly Stack<IUndoableCommand> _redo = new();

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        public void Execute(IUndoableCommand command)
        {
            command.Execute();

            _undo.Push(command);

            _redo.Clear();
        }

        public void Undo()
        {
            if (_undo.Count == 0)
                return;

            var cmd = _undo.Pop();

            cmd.Undo();

            _redo.Push(cmd);
        }

        public void Redo()
        {
            if (_redo.Count == 0)
                return;

            var cmd = _redo.Pop();

            cmd.Execute();

            _undo.Push(cmd);
        }
    }
}

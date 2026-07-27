using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Interfaces
{
    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
    }
}

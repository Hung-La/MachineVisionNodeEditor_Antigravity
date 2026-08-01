using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MachineVisionNodeEditor.Interfaces
{
    internal interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object? parameter);
    }
}

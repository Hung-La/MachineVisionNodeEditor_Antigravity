using MachineVisionNodeEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MachineVisionNodeEditor.Commands
{
    public class AsyncRelayCommand : IAsyncCommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;

        private bool _isExecuting;

        public AsyncRelayCommand(
            Func<Task> execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting &&
                   (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Predicate<T?>? _canExecute;

        public AsyncRelayCommand(
            Func<T?, Task> execute,
            Predicate<T?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ||
                   _canExecute((T?)parameter);
        }

        public async void Execute(object? parameter)
        {
            await _execute((T?)parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }
}

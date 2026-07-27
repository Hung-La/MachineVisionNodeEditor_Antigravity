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
    public class ConnectCommand : IUndoableCommand
    {
        private readonly Window_MainWindowViewModel _diagram;

        private readonly Node_ConnectionViewModel _connection;

        public ConnectCommand(Window_MainWindowViewModel diagram,Node_ConnectionViewModel connection)
        {
            _diagram = diagram;
            _connection = connection;
        }

        public void Execute()
        {
            if (!_diagram.Connections.Contains(_connection))
            {
                _diagram.Connections.Add(_connection);

                _connection.ConnectionModel.FromPort.IsConnected = true;
                _connection.ConnectionModel.ToPort.IsConnected = true;

                // ✅ thêm 2 dòng này (phòng khi Redo sau khi đã Undo)
                if (!_connection.ConnectionModel.FromPort.Connections.Contains(_connection.ConnectionModel))
                    _connection.ConnectionModel.FromPort.Connections.Add(_connection.ConnectionModel);
                if (!_connection.ConnectionModel.ToPort.Connections.Contains(_connection.ConnectionModel))
                    _connection.ConnectionModel.ToPort.Connections.Add(_connection.ConnectionModel);


                _connection.ConnectionModel.UpdateControls();
            }
        }

        public void Undo()
        {
            _diagram.Connections.Remove(_connection);

            _connection.ConnectionModel.FromPort.IsConnected = false;
            _connection.ConnectionModel.ToPort.IsConnected = false;

            _connection.ConnectionModel.FromPort.Connections.Remove(_connection.ConnectionModel);
            _connection.ConnectionModel.ToPort.Connections.Remove(_connection.ConnectionModel);
        }
    }
}

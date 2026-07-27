using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Services.NodeServies
{
    public class DisconnectCommand : IUndoableCommand
    {
        private readonly Window_MainWindowViewModel _diagram;

        // Connection mà user chọn để xóa
        private readonly List<Node_ConnectionViewModel> _selectedConnections;

        // Node sẽ bị xóa
        private readonly List<NodeControl_NodeViewModel> _removedNodes;

        // Connection bị xóa do node bị xóa
        private readonly List<Node_ConnectionViewModel> _removedConnections = new();

        public DisconnectCommand(
            Window_MainWindowViewModel diagram,
            List<Node_ConnectionViewModel> selectedConnections,
            List<NodeControl_NodeViewModel> removedNodes)
        {
            _diagram = diagram;

            _selectedConnections = selectedConnections ?? new();

            _removedNodes = removedNodes ?? new();
        }

        public void Execute()
        {
            _removedConnections.Clear();

            //---------------------------------
            // 1. Xóa connection được chọn
            //---------------------------------

            foreach (var connection in _selectedConnections)
            {
                if (_diagram.Connections.Remove(connection))
                {
                    connection.ConnectionModel.FromPort.IsConnected = false;
                    connection.ConnectionModel.ToPort.IsConnected = false;

                    connection.ConnectionModel.FromPort.Connections.Remove(connection.ConnectionModel);
                    connection.ConnectionModel.ToPort.Connections.Remove(connection.ConnectionModel);
                }
            }

            //---------------------------------
            // 2. Xóa connection của node bị xóa
            //---------------------------------

            foreach (var node in _removedNodes)
            {
                RemoveConnections(node.NodeModel.InputPorts);

                RemoveConnections(node.NodeModel.OutputPorts);
            }
        }

        /// <summary>
        /// Xóa toàn bộ connection của danh sách port
        /// </summary>
        private void RemoveConnections(IEnumerable<Node_PortViewModel> ports)
        {
            foreach (var port in ports)
            {
                // ToList để tránh lỗi modify collection khi đang foreach
                foreach (var model in port.PortModel.Connections.ToList())
                {
                    var vm = _diagram.Connections
                                     .FirstOrDefault(c => c.ConnectionModel == model);

                    // Connection có thể đã bị xóa trước đó
                    if (vm == null)
                        continue;

                    if (!_removedConnections.Contains(vm))
                        _removedConnections.Add(vm);

                    _diagram.Connections.Remove(vm);

                    vm.ConnectionModel.FromPort.IsConnected = false;
                    vm.ConnectionModel.ToPort.IsConnected = false;

                    vm.ConnectionModel.FromPort.Connections.Remove(vm.ConnectionModel);
                    vm.ConnectionModel.ToPort.Connections.Remove(vm.ConnectionModel);
                }
            }
        }

        public void Undo()
        {
            //---------------------------------
            // 1. Khôi phục connection do node bị xóa
            //---------------------------------

            foreach (var connection in _removedConnections)
            {
                if (!_diagram.Connections.Contains(connection))
                {
                    _diagram.Connections.Add(connection);

                    connection.ConnectionModel.FromPort.IsConnected = true;
                    connection.ConnectionModel.ToPort.IsConnected = true;

                    connection.ConnectionModel.FromPort.Connections.Add(connection.ConnectionModel);
                    connection.ConnectionModel.ToPort.Connections.Add(connection.ConnectionModel);

                    connection.ConnectionModel.UpdateControls();
                }
            }

            //---------------------------------
            // 2. Khôi phục connection user chọn xóa
            //---------------------------------

            foreach (var connection in _selectedConnections)
            {
                if (!_diagram.Connections.Contains(connection))
                {
                    _diagram.Connections.Add(connection);

                    connection.ConnectionModel.FromPort.IsConnected = true;
                    connection.ConnectionModel.ToPort.IsConnected = true;

                    connection.ConnectionModel.FromPort.Connections.Add(connection.ConnectionModel);
                    connection.ConnectionModel.ToPort.Connections.Add(connection.ConnectionModel);

                    connection.ConnectionModel.UpdateControls();
                }
            }

            //_diagram.UndoRedoService.Undo();

            _removedConnections.Clear();
        }
    }
}

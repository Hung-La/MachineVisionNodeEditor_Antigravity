using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodePropertyModels;
using MachineVisionNodeEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodeOperationModels
{
    public abstract class NodeOperationModel<TProperty>
        : BaseViewModel,
          INodeOperation<TProperty>
        where TProperty : NodePropertyModel
    {

        public void Execute()
        {
            throw new InvalidOperationException(
            "Use Execute(property)");
        }

        public abstract void Execute(TProperty property);
    }
}

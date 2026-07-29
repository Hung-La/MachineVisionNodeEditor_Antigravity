using MachineVisionNodeEditor.Models.NodePropertyModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Interfaces.NodeInterfaces
{
    public interface INodeOperation
    {
        void Execute();
    }
    interface INodeOperation<in TProperty> : INodeOperation
        where TProperty : NodePropertyModel
    {
        void Execute(TProperty property);
    }
}

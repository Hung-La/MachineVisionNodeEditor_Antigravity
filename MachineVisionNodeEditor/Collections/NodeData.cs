using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Collections
{
    public abstract class NodeData
    {
        public string Name { get; }

        public Type DataType { get; }

        public object? Value { get; set; }

        protected NodeData(
            string name,
            Type type,
            object? value = null)
        {
            Name = name;
            DataType = type;
            Value = value;
        }
    }

    public class NodeData<T> : NodeData
    {
        public new T Value
        {
            get => base.Value is T val ? val : default!;
            set => base.Value = value;
        }

        public NodeData(
            string name,
            T value)
            : base(name, typeof(T), value)
        {

        }
    }
}

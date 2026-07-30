using MachineVisionNodeEditor.Collections;
using OpenCvSharp;
using System.Collections.Generic;

namespace MachineVisionNodeEditor.Models.NodeContextModels
{
    public class NodeContext
    {
        public Dictionary<string, NodeData> Inputs { get; } = new();

        public Dictionary<string, NodeData> Outputs { get; } = new();

        public Mat? InputImage
        {
            get => Get<Mat>("Image");
            set
            {
                if (Inputs.TryGetValue("Image", out var data))
                {
                    data.Value = value;
                }
                else
                {
                    Inputs["Image"] = new NodeData<Mat>("Image", value!);
                }
            }
        }

        public Mat? OutputImage
        {
            get => GetOutput<Mat>("Image");
            set => Set("Image", value!);
        }

        public T? Get<T>(string name)
        {
            if (Inputs.TryGetValue(name, out var data))
            {
                if (data is NodeData<T> typedData)
                {
                    return typedData.Value;
                }
                if (data.Value is T val)
                {
                    return val;
                }
            }
            return default;
        }

        public T? GetOutput<T>(string name)
        {
            if (Outputs.TryGetValue(name, out var data))
            {
                if (data is NodeData<T> typedData)
                {
                    return typedData.Value;
                }
                if (data.Value is T val)
                {
                    return val;
                }
            }
            return default;
        }

        public void Set<T>(string name, T value)
        {
            if (Outputs.TryGetValue(name, out var data))
            {
                data.Value = value;
            }
            else
            {
                Outputs[name] = new NodeData<T>(name, value);
            }
        }
    }
}

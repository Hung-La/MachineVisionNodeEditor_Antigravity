using MachineVisionNodeEditor.Collections;
using MachineVisionNodeEditor.ViewModels;
using OpenCvSharp;
using System.Collections.Generic;

namespace MachineVisionNodeEditor.Models.NodeContextModels
{
    public class NodeContext : BaseViewModel
    {
        public Dictionary<string, NodeData> Inputs { get; } = new();

        public Dictionary<string, NodeData> Outputs { get; } = new();

        private List<Mat> _inputImages = new();
        public List<Mat> InputImages
        {
            get => _inputImages;
            set { SetField(ref _inputImages, value); OnPropertyChanged(nameof(InputImages)); }
        }

        private List<Mat> _outputImages = new();
        public List<Mat> OutputImages
        {
            get => _outputImages;
            set { SetField(ref _outputImages, value); OnPropertyChanged(nameof(OutputImages)); }
        }

        public Mat? InputImage
        {
            get => Get<Mat>("Image");
            set => SetInput("Image", value!);
        }

        public Mat? OutputImage
        {
            get => GetOutput<Mat>("Image");
            set => Set("Image", value!);
        }

        public void SetInput<T>(string name, T value)
        {
            if (Inputs.TryGetValue(name, out var data))
            {
                data.Value = value;
            }
            else
            {
                Inputs[name] = new NodeData<T>(name, value);
            }
            if (name == "Image")
            {
                OnPropertyChanged(nameof(InputImage));
            }
            OnPropertyChanged(nameof(Inputs));
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
            if (name == "Image")
            {
                OnPropertyChanged(nameof(OutputImage));
            }
            OnPropertyChanged(nameof(Outputs));
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
    }
}

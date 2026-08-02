using OpenCvSharp;
using System;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ImageResize_NodePropertyModel : NodePropertyModel
    {
        private int _targetWidth = 640;
        private int _targetHeight = 480;
        private double _scaleX = 1.0;
        private double _scaleY = 1.0;
        private bool _useScaleFactor = false;
        private InterpolationFlags _interpolation = InterpolationFlags.Linear;

        public int TargetWidth
        {
            get => _targetWidth;
            set => SetField(ref _targetWidth, Math.Max(1, value));
        }

        public int TargetHeight
        {
            get => _targetHeight;
            set => SetField(ref _targetHeight, Math.Max(1, value));
        }

        public double ScaleX
        {
            get => _scaleX;
            set => SetField(ref _scaleX, Math.Max(0.01, value));
        }

        public double ScaleY
        {
            get => _scaleY;
            set => SetField(ref _scaleY, Math.Max(0.01, value));
        }

        public bool UseScaleFactor
        {
            get => _useScaleFactor;
            set => SetField(ref _useScaleFactor, value);
        }

        public InterpolationFlags Interpolation
        {
            get => _interpolation;
            set => SetField(ref _interpolation, value);
        }

        public ObservableCollection<InterpolationFlags> Interpolations { get; } = new(Enum.GetValues<InterpolationFlags>());
    }
}

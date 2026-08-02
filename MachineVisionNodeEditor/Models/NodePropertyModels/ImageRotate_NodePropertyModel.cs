using OpenCvSharp;
using System;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ImageRotate_NodePropertyModel : NodePropertyModel
    {
        private double _angle = 90.0;
        private double _scale = 1.0;
        private InterpolationFlags _interpolation = InterpolationFlags.Linear;
        private BorderTypes _borderMode = BorderTypes.Constant;

        public double Angle
        {
            get => _angle;
            set => SetField(ref _angle, value);
        }

        public double Scale
        {
            get => _scale;
            set => SetField(ref _scale, Math.Max(0.01, value));
        }

        public InterpolationFlags Interpolation
        {
            get => _interpolation;
            set => SetField(ref _interpolation, value);
        }

        public BorderTypes BorderMode
        {
            get => _borderMode;
            set => SetField(ref _borderMode, value);
        }

        public ObservableCollection<InterpolationFlags> Interpolations { get; } = new(Enum.GetValues<InterpolationFlags>());
        public ObservableCollection<BorderTypes> BorderModes { get; } = new(Enum.GetValues<BorderTypes>());
    }
}

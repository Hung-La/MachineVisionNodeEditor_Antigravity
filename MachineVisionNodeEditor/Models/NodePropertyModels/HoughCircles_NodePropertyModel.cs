using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class HoughCircles_NodePropertyModel : NodePropertyModel
    {
        private double _dp = 1.0;
        private double _minDist = 20.0;
        private double _param1 = 100.0;
        private double _param2 = 30.0;
        private int _minRadius = 0;
        private int _maxRadius = 0;
        private string _selectedColor = "Cyan";
        private byte _customR = 0;
        private byte _customG = 255;
        private byte _customB = 255;
        private int _thickness = 2;
        private int _totalCircles = 0;
        private CircleSegment[]? _circles;

        public double Dp
        {
            get => _dp;
            set => SetField(ref _dp, System.Math.Max(0.1, value));
        }

        public double MinDist
        {
            get => _minDist;
            set => SetField(ref _minDist, System.Math.Max(1.0, value));
        }

        public double Param1
        {
            get => _param1;
            set => SetField(ref _param1, System.Math.Max(1.0, value));
        }

        public double Param2
        {
            get => _param2;
            set => SetField(ref _param2, System.Math.Max(1.0, value));
        }

        public int MinRadius
        {
            get => _minRadius;
            set => SetField(ref _minRadius, System.Math.Max(0, value));
        }

        public int MaxRadius
        {
            get => _maxRadius;
            set => SetField(ref _maxRadius, System.Math.Max(0, value));
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set => SetField(ref _selectedColor, value);
        }

        public byte CustomR
        {
            get => _customR;
            set => SetField(ref _customR, value);
        }

        public byte CustomG
        {
            get => _customG;
            set => SetField(ref _customG, value);
        }

        public byte CustomB
        {
            get => _customB;
            set => SetField(ref _customB, value);
        }

        public int Thickness
        {
            get => _thickness;
            set => SetField(ref _thickness, System.Math.Max(1, value));
        }

        public int TotalCircles
        {
            get => _totalCircles;
            set => SetField(ref _totalCircles, value);
        }

        public CircleSegment[]? Circles
        {
            get => _circles;
            set => SetField(ref _circles, value);
        }

        public ObservableCollection<string> AvailableColors { get; } = new()
        {
            "Cyan",
            "Green",
            "Red",
            "Blue",
            "Yellow",
            "Magenta",
            "White",
            "Black",
            "Orange",
            "Custom"
        };

        public Scalar GetScalarColor()
        {
            return SelectedColor switch
            {
                "Red" => new Scalar(0, 0, 255),
                "Green" => new Scalar(0, 255, 0),
                "Blue" => new Scalar(255, 0, 0),
                "Yellow" => new Scalar(0, 255, 255),
                "Cyan" => new Scalar(255, 255, 0),
                "Magenta" => new Scalar(255, 0, 255),
                "White" => new Scalar(255, 255, 255),
                "Black" => new Scalar(0, 0, 0),
                "Orange" => new Scalar(0, 165, 255),
                "Custom" => new Scalar(CustomB, CustomG, CustomR),
                _ => new Scalar(255, 255, 0)
            };
        }
    }
}

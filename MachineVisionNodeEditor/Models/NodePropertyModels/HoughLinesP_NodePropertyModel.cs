using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class HoughLinesP_NodePropertyModel : NodePropertyModel
    {
        private double _rho = 1.0;
        private double _theta = 1.0; // In degrees
        private int _threshold = 50;
        private double _minLineLength = 50.0;
        private double _maxLineGap = 10.0;
        private string _selectedColor = "Red";
        private byte _customR = 255;
        private byte _customG = 0;
        private byte _customB = 0;
        private int _thickness = 2;
        private int _totalLines = 0;
        private LineSegmentPoint[]? _lines;

        public double Rho
        {
            get => _rho;
            set => SetField(ref _rho, System.Math.Max(0.1, value));
        }

        public double Theta
        {
            get => _theta;
            set => SetField(ref _theta, System.Math.Max(0.1, value));
        }

        public int Threshold
        {
            get => _threshold;
            set => SetField(ref _threshold, System.Math.Max(1, value));
        }

        public double MinLineLength
        {
            get => _minLineLength;
            set => SetField(ref _minLineLength, System.Math.Max(0.0, value));
        }

        public double MaxLineGap
        {
            get => _maxLineGap;
            set => SetField(ref _maxLineGap, System.Math.Max(0.0, value));
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

        public int TotalLines
        {
            get => _totalLines;
            set => SetField(ref _totalLines, value);
        }

        public LineSegmentPoint[]? Lines
        {
            get => _lines;
            set => SetField(ref _lines, value);
        }

        public ObservableCollection<string> AvailableColors { get; } = new()
        {
            "Red",
            "Green",
            "Blue",
            "Yellow",
            "Cyan",
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
                _ => new Scalar(0, 0, 255)
            };
        }
    }
}

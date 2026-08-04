using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class FilterContours_NodePropertyModel : NodePropertyModel
    {
        private double _areaMin = 50;
        private double _areaMax = 50000;
        private string _selectedColor = "Green";
        private byte _customR = 0;
        private byte _customG = 255;
        private byte _customB = 0;
        private int _thickness = 2;
        private int _totalContours = 0;
        private int _drawnContours = 0;
        private Point[][] _contours;

        public double AreaMin
        {
            get => _areaMin;
            set => SetField(ref _areaMin, System.Math.Clamp(value, 0, AreaMax));
        }

        public double AreaMax
        {
            get => _areaMax;
            set { SetField(ref _areaMax, System.Math.Clamp(value, 0, 8000)); OnPropertyChanged(nameof(AreaMin)); }
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

        public int TotalContours
        {
            get => _totalContours;
            set => SetField(ref _totalContours, value);
        }

        public int DrawnContours
        {
            get => _drawnContours;
            set => SetField(ref _drawnContours, value);
        }

        public ObservableCollection<string> AvailableColors { get; } = new()
        {
            "Green",
            "Red",
            "Blue",
            "Yellow",
            "Cyan",
            "Magenta",
            "White",
            "Black",
            "Orange",
            "Custom"
        };

        public Point[][] Contours
        {
            get => _contours;
            set => SetField(ref _contours, value);
        }

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
                _ => new Scalar(0, 255, 0)
            };
        }
    }
}

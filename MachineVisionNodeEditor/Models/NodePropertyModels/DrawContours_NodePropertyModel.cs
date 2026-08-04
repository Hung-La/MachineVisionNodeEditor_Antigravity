using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class DrawContours_NodePropertyModel : NodePropertyModel
    {
        private Point[][]? _contours;
        private Scalar _color;
        private int _thickness;
        public Point[][] Contours
        {
            get => _contours;
            set { SetField(ref _contours, value); }
        }

        public Scalar Color
        {
            get => _color;
            set { SetField(ref _color, value); }
        }

        public int Thickness 
        {
            get => _thickness;
            set { SetField(ref _thickness, value); }
        }
    }
}

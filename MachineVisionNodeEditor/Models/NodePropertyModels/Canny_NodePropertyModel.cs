using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class Canny_NodePropertyModel : NodePropertyModel
    {
        private double _threshold1 = 100;
        private double _threshold2 = 200;
        private int _apertureSize = 3;
        private bool _l2Gradient = false;

        public double Threshold1
        {
            get => _threshold1;
            set => SetField(ref _threshold1, value);
        }

        public double Threshold2
        {
            get => _threshold2;
            set => SetField(ref _threshold2, value);
        }

        public int ApertureSize
        {
            get => _apertureSize;
            set => SetField(ref _apertureSize, value);
        }

        public bool L2Gradient
        {
            get => _l2Gradient;
            set => SetField(ref _l2Gradient, value);
        }

        public ObservableCollection<int> ApertureSizes { get; } = new() { 3, 5, 7 };
    }
}

using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class Threshold_NodePropertyModel : NodePropertyModel
    {
        private ThresholdTypes _selectedType = OpenCvSharp.ThresholdTypes.Binary;
        private double _thresh = 128;
        private double _maxVal = 255;

        public ThresholdTypes SelectedType
        {
            get => _selectedType;
            set => SetField(ref _selectedType, value);
        }

        public double Thresh
        {
            get => _thresh;
            set => SetField(ref _thresh, value);
        }

        public double MaxVal
        {
            get => _maxVal;
            set => SetField(ref _maxVal, value);
        }

        public ObservableCollection<ThresholdTypes> AvailableTypes { get; } = new(System.Enum.GetValues<ThresholdTypes>());
    }
}

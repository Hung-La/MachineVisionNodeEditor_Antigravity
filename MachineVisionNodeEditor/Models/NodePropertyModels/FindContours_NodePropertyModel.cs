using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class FindContours_NodePropertyModel : NodePropertyModel
    {
        private RetrievalModes _mode = RetrievalModes.External;
        private ContourApproximationModes _method = ContourApproximationModes.ApproxSimple;
        private int _thickness = 2;
        private int _contourCount = 0;

        public RetrievalModes Mode
        {
            get => _mode;
            set => SetField(ref _mode, value);
        }

        public ContourApproximationModes Method
        {
            get => _method;
            set => SetField(ref _method, value);
        }

        public int Thickness
        {
            get => _thickness;
            set => SetField(ref _thickness, System.Math.Max(1, value));
        }

        public int ContourCount
        {
            get => _contourCount;
            set => SetField(ref _contourCount, value);
        }

        public ObservableCollection<RetrievalModes> Modes { get; } = new(System.Enum.GetValues<RetrievalModes>());
        public ObservableCollection<ContourApproximationModes> Methods { get; } = new(System.Enum.GetValues<ContourApproximationModes>());
    }
}

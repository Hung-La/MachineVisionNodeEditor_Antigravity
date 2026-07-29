using OpenCvSharp;
using System.Collections.ObjectModel;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class Erode_NodePropertyModel : NodePropertyModel
    {
        private MorphShapes _shape = MorphShapes.Rect;
        private int _kSize = 3;
        private int _iterations = 1;

        public MorphShapes Shape
        {
            get => _shape;
            set => SetField(ref _shape, value);
        }

        public int KSize
        {
            get => _kSize;
            set
            {
                int val = System.Math.Max(1, value);
                if (val % 2 == 0) val += 1;
                SetField(ref _kSize, val);
            }
        }

        public int Iterations
        {
            get => _iterations;
            set => SetField(ref _iterations, System.Math.Max(1, value));
        }

        public ObservableCollection<MorphShapes> Shapes { get; } = new(System.Enum.GetValues<MorphShapes>());
    }
}

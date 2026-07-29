namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class GaussianBlur_NodePropertyModel : NodePropertyModel
    {
        private int _kSize = 3;
        private double _sigmaX = 0;

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

        public double SigmaX
        {
            get => _sigmaX;
            set => SetField(ref _sigmaX, value);
        }
    }
}

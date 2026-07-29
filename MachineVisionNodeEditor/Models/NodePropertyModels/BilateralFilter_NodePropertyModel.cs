namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class BilateralFilter_NodePropertyModel : NodePropertyModel
    {
        private int _d = 9;
        private double _sigmaColor = 75;
        private double _sigmaSpace = 75;

        public int D
        {
            get => _d;
            set => SetField(ref _d, value);
        }

        public double SigmaColor
        {
            get => _sigmaColor;
            set => SetField(ref _sigmaColor, value);
        }

        public double SigmaSpace
        {
            get => _sigmaSpace;
            set => SetField(ref _sigmaSpace, value);
        }
    }
}

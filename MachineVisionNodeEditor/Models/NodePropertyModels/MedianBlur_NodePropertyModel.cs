namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class MedianBlur_NodePropertyModel : NodePropertyModel
    {
        private int _kSize = 3;

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
    }
}

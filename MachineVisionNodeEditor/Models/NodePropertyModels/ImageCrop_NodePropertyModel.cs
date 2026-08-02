using System;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ImageCrop_NodePropertyModel : NodePropertyModel
    {
        private int _cropX = 0;
        private int _cropY = 0;
        private int _cropWidth = 200;
        private int _cropHeight = 200;

        public int CropX
        {
            get => _cropX;
            set => SetField(ref _cropX, Math.Max(0, value));
        }

        public int CropY
        {
            get => _cropY;
            set => SetField(ref _cropY, Math.Max(0, value));
        }

        public int CropWidth
        {
            get => _cropWidth;
            set => SetField(ref _cropWidth, Math.Max(1, value));
        }

        public int CropHeight
        {
            get => _cropHeight;
            set => SetField(ref _cropHeight, Math.Max(1, value));
        }
    }
}

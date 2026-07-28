using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.Views.NodeProperties;
using OpenCvSharp;
using System.Collections.Generic;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class NodePropertyModel : BaseViewModel
    {
        #region Fields
        private string _name;
        private string _description;
        private bool _gridEnabled;
        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set { SetField(ref _name, value); }
        }


        public string Description
        {
            get => _description;
            set
            {
                SetField(ref _description, value);
            }
        }


        public bool GridEnabled
        {
            get => _gridEnabled;
            set { SetField(ref _gridEnabled, value); }
        }

        private int _width;
        public int Width
        {
            get => _width;
            set { SetField(ref _width, value); }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set { SetField(ref _height, value); }
        }

        public Dictionary<string, object> Inputs { get; } = new();

        public Dictionary<string, object> Outputs { get; } = new();

        /// <summary>
        /// Typed accessor cho XAML binding — đọc/ghi Inputs["Image"] và tự raise PropertyChanged.
        /// </summary>
        public Mat InputImage
        {
            get => Inputs.TryGetValue("Image", out var v) ? v as Mat : null;
            set
            {
                Inputs["Image"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Typed accessor cho XAML binding — đọc/ghi Outputs["Image"] và tự raise PropertyChanged.
        /// </summary>
        public Mat OutputImage
        {
            get => Outputs.TryGetValue("Image", out var v) ? v as Mat : null;
            set
            {
                Outputs["Image"] = value;
                OnPropertyChanged();
            }
        }

        public NodePropertyControl View
        {
            set; get;
        }

        #endregion


    }


}

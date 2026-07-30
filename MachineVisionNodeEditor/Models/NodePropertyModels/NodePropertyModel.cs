using MachineVisionNodeEditor.Collections;
using MachineVisionNodeEditor.Models.NodeContextModels;
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

        public NodeContext Context { get; } = new();

        /// <summary>
        /// Typed accessor cho XAML binding — đọc/ghi Inputs["Image"] và tự raise PropertyChanged.
        /// </summary>
        public Mat InputImage
        {
            get => Context.InputImage;
            set
            {
                Context.InputImage = value;
                if (value != null && !value.IsDisposed && value.Width > 0 && value.Height > 0)
                {
                    Width = value.Width;
                    Height = value.Height;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Typed accessor cho XAML binding — đọc/ghi Outputs["Image"] và tự raise PropertyChanged.
        /// </summary>
        public Mat OutputImage
        {
            get => Context.OutputImage;
            set
            {
                Context.OutputImage = value;
                if (value != null && !value.IsDisposed && value.Width > 0 && value.Height > 0)
                {
                    Width = value.Width;
                    Height = value.Height;
                }
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

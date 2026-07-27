using MachineVisionNodeEditor.Converters;
using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.Views.NodeProperties;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class NodePropertyModel : BaseViewModel
    {
        #region Fields
        private string _name;
        private string _description;
        private Mat _sourceImage;
        private Mat _destinationImage;
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

        public Mat SourceImage
        {
            get => _sourceImage;
            set
            {
                SetField(ref _sourceImage, value);
                var converter = new ResolutionConverter();
                Width = (int)converter.Convert(
                                               value,
                                               typeof(int),
                                               "Width",
                                               CultureInfo.CurrentCulture);
                Height = (int)converter.Convert(
                                               value,
                                               typeof(int),
                                               "Height",
                                               CultureInfo.CurrentCulture);
            }
        }

        public Mat DestinationImage
        {
            get => _destinationImage;
            set { SetField(ref _destinationImage, value); }
        }

        public NodePropertyControl View
        {
            set; get;
        }

        #endregion


    }


}

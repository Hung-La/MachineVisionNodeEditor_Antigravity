using MachineVisionAlgorithm.ImageInteraction;
using MachineVisionNodeEditor.Converters;
using MachineVisionNodeEditor.ViewModels;
using MachineVisionNodeEditor.Views.Windows.NodeWindows;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ImageImport_NodePropertyModel : NodePropertyModel
    {
        #region Fields
        private string _filePath;
        private ColorMode? _selectedMode;

        #endregion

        #region Properties
        public string FilePath
        {
            get => _filePath;
            set
            {
                SetField(ref _filePath, value);
                SourceImage = Cv2.ImRead(value);
                //var converter = new ResolutionConverter();
                //Width = (int)converter.Convert(
                //                               FilePath,
                //                               typeof(int),
                //                               "Width",
                //                               CultureInfo.CurrentCulture);
                //Height = (int)converter.Convert(
                //                               FilePath,
                //                               typeof(int),
                //                               "Height",
                //                               CultureInfo.CurrentCulture);

            }
        }


        public ColorMode? SelectedMode
        {
            get => _selectedMode;
            set 
            {
                SetField(ref _selectedMode, value);
                if (File.Exists(FilePath))
                {
                    if (value == null)
                    {
                        DestinationImage = ImageImport.ReadImage(FilePath);
                    }
                    else
                    {
                        DestinationImage = ImageImport.ReadImage(FilePath, (ImreadModes)value);
                    }

                }
            }
        }

        #endregion
        public ObservableCollection<ColorMode> ColorModes { get; } = new(typeof(ColorMode)
                                                                         .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                                                         .Select(f => (ColorMode)f.GetValue(null)!));

        public ImageImport_NodePropertyModel()
        {
            //foreach (ImreadModes mode in Enum.GetValues(typeof(ColorMode)))
            //{
            //    ColorModes.Add(mode);
            //}

        }

    }

    public enum ColorMode
    {
        [Description("Unchanged")]
        Unchanged = ImreadModes.Unchanged,

        [Description("Gray")]
        Graysacle = ImreadModes.Grayscale,

        [Description("Color")]
        Color = ImreadModes.Color,

        [Description("Any Depth")]
        AnyDepth = ImreadModes.AnyDepth


    }
}

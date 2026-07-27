using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace MachineVisionNodeEditor.Models.NodePropertyModels
{
    public class ConvertColor_NodePropertyModel : NodePropertyModel
    {
        #region Fields
        private ColorCode? _selectedCode;

        #endregion

        #region Properties

        public ColorCode? SelectedCode
        {
            get => _selectedCode;
            set { SetField(ref _selectedCode, value); }
        }

        #endregion

        public ObservableCollection<ColorCode> ColorCodes { get; } = new(typeof(ColorCode)
                                                                 .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                                                 .Select(f => (ColorCode)f.GetValue(null)!));
    }

    public enum ColorCode
    {
        [Description("BGR → Gray")]
        BGR2GRAY = ColorConversionCodes.BGR2GRAY,

        [Description("Gray → BGR")]
        GRAY2BGR = ColorConversionCodes.GRAY2BGR,

        [Description("BGR → RGB")]
        BGR2RGB = ColorConversionCodes.BGR2RGB,

        [Description("RGB → BGR")]
        RGB2BGR = ColorConversionCodes.RGB2BGR,

        [Description("BGR → HSV")]
        BGR2HSV = ColorConversionCodes.BGR2HSV,

        [Description("HSV → BGR")]
        HSV2BGR = ColorConversionCodes.HSV2BGR,

        [Description("BGR → HLS")]
        BGR2HLS = ColorConversionCodes.BGR2HLS,

        [Description("HLS → BGR")]
        HLS2BGR = ColorConversionCodes.HLS2BGR,

        [Description("BGR → Lab")]
        BGR2Lab = ColorConversionCodes.BGR2Lab,

        [Description("Lab → BGR")]
        Lab2BGR = ColorConversionCodes.Lab2BGR,

        [Description("BGR → Luv")]
        BGR2Luv = ColorConversionCodes.BGR2Luv,

        [Description("Luv → BGR")]
        Luv2BGR = ColorConversionCodes.Luv2BGR,

        [Description("BGR → XYZ")]
        BGR2XYZ = ColorConversionCodes.BGR2XYZ,

        [Description("XYZ → BGR")]
        XYZ2BGR = ColorConversionCodes.XYZ2BGR,

        [Description("BGR → YCrCb")]
        BGR2YCrCb = ColorConversionCodes.BGR2YCrCb,

        [Description("YCrCb → BGR")]
        YCrCb2BGR = ColorConversionCodes.YCrCb2BGR,

        [Description("BGR → BGRA")]
        BGR2BGRA = ColorConversionCodes.BGR2BGRA,

        [Description("BGRA → BGR")]
        BGRA2BGR = ColorConversionCodes.BGRA2BGR,

        [Description("BGR → RGBA")]
        BGR2RGBA = ColorConversionCodes.BGR2RGBA,

        [Description("RGBA → BGR")]
        RGBA2BGR = ColorConversionCodes.RGBA2BGR,

        [Description("BGRA → Gray")]
        BGRA2GRAY = ColorConversionCodes.BGRA2GRAY,

        [Description("RGBA → Gray")]
        RGBA2GRAY = ColorConversionCodes.RGBA2GRAY
    }
}

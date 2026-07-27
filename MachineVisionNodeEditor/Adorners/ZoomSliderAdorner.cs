using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace MachineVisionNodeEditor.Adorners
{
    public class ZoomSliderAdorner : Adorner
    {
        public string Text { get; set; } = "";
        public bool IsVisible { get; set; }

        private readonly Border border;
        private readonly TextBlock textBlock;

        public ZoomSliderAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;

            textBlock = new TextBlock();

            border = new Border
            {
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2),
                Child = textBlock
            };

            //border.SetResourceReference(
            //    Border.BackgroundProperty,
            //    "PopupBackground");

            //textBlock.SetResourceReference(
            //    TextElement.ForegroundProperty,
            //    "PopupForeground");

            textBlock.SetResourceReference(
                TextElement.FontFamilyProperty,
                "MaterialDesignFont");
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!IsVisible)
                return;


            Slider slider = (Slider)AdornedElement;

            //Brush background = border.Background;
            //Brush foreground = textBlock.Foreground;

            Brush background =
                (Brush)Application.Current.FindResource("PopupBackground");

            Brush foreground =
                (Brush)Application.Current.FindResource("PopupForeground");

            //FontFamily font =
            //    (FontFamily)Application.Current.FindResource("MaterialDesignFont");

            var formatted = new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily,
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal),
                14,
                foreground,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            Rect rect = new Rect(
                -60,
                slider.ActualHeight / 2 - 18,
                50,
                36);

            dc.DrawRoundedRectangle(
                background,
                null,
                rect,
                5,
                5);

            dc.DrawText(formatted,
                new Point(
                    rect.Left + 8,
                    rect.Top + 9));
        }


    }
}

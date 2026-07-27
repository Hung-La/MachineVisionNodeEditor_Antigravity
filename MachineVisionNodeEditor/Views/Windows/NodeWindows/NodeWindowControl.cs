using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MachineVisionNodeEditor.Views.Windows.NodeWindows
{
    public class NodeWindowControl : Window
    {
        public NodeWindowControl() 
        {
            Loaded += NodeWindowControl_Loaded;
        }

        protected void NodeWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            FitImage();
        }

        protected void NodeWindowControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void FitImage()
        {

            //if (Image.Source == null)
            //    return;

            //if (Image.ActualWidth <= 0 || Image.ActualHeight <= 0)
            //    return;

            //double viewportWidth = Viewport.ActualWidth;
            //double viewportHeight = Viewport.ActualHeight;

            //double scaleX = viewportWidth / Image.ActualWidth;
            //double scaleY = viewportHeight / Image.ActualHeight;

            //double s = Math.Min(scaleX, scaleY);

            //scale.ScaleX = s;
            //scale.ScaleY = s;

            //translate.X =
            //    (viewportWidth - Image.ActualWidth * s) / 2;

            //translate.Y =
            //    (viewportHeight - Image.ActualHeight * s) / 2;
        }
    }
}

using MachineVisionNodeEditor.Adorners;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace MachineVisionNodeEditor.Behaviors
{
    public class ZoomSliderAdornerBehavior : Behavior<Slider>
    {
        private ZoomSliderAdorner? _adorner;

        private DispatcherTimer? _timer;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += Loaded;
            AssociatedObject.ValueChanged += ValueChanged;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

            _adorner = new ZoomSliderAdorner(AssociatedObject);

            layer.Add(_adorner);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1.5);
            _timer.Tick += TimerTick;
        }

        private void ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e)
        {
            if (_adorner == null)
                return;
            _adorner.Text = $"{e.NewValue:F1}x";
            _adorner.IsVisible = true;
            _adorner.InvalidateVisual();
            _timer.Stop();
            _timer.Start();
        }

        private void TimerTick(object sender,EventArgs e)
        {
            _timer.Stop();
            _adorner.IsVisible = false;
            _adorner.InvalidateVisual();
        }
    }
}

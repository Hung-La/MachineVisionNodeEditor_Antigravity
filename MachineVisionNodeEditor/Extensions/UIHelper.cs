using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using MachineVisionNodeEditor.ViewModels.WindowViewModels;
using MachineVisionNodeEditor.Views.Nodes;
using MachineVisionNodeEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace MachineVisionNodeEditor.Extensions
{
    public class UIHelper
    {
        /// <summary>
        /// Trả về toạ độ tâm của <paramref name="element"/> 
        /// tính theo hệ toạ độ của <paramref name="relativeTo"/>.
        /// </summary>
        public static Point GetCenter(FrameworkElement element, Visual relativeTo)
        {
            var transform = element.TransformToVisual(relativeTo);
            var topLeft = transform.Transform(new Point(0, 0));
            return new Point(
                topLeft.X + element.ActualWidth / 2,
                topLeft.Y + element.ActualHeight / 2);
        }

        /// <summary>
        /// Duyệt Visual Tree để tìm child đầu tiên kiểu T.
        /// </summary>
        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Duyệt Visual Tree lên trên để tìm ancestor kiểu T.
        /// </summary>
        public static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T t) return t;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static FrameworkElement? FindPortElement(PortModel port)
        {

            foreach (var item in MainWindow.Instance.NodesControl.Items)
            {
                var container = MainWindow.Instance.NodesControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (container == null) continue;

                var nodeView = FindVisualChild<Node_NodeView>(container);
                if (nodeView?.DataContext is NodeControl_NodeViewModel nodeViewModel && nodeViewModel.NodeModel == port.Owner)
                {
                    return nodeView.GetPortElement(port);
                }

            }

            return null;
        }

        public static FrameworkElement? FindPortElement(Node_NodeView node)
        {
            foreach (var item in MainWindow.Instance.NodesControl.Items)
            {
                var container = MainWindow.Instance.NodesControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (container == null) continue;

                var nodeView = FindVisualChild<Node_NodeView>(container);
                if (nodeView?.DataContext is Node_NodeViewModel nodeViewModel && nodeViewModel == node.DataContext)
                {
                    return FindVisualChild<Node_PortView>(node);
                }
            }

            return null;
        }

        public static T? FindNodeElement<T>(Node_PortView portView) where T : FrameworkElement
        {

            foreach (var item in MainWindow.Instance.NodesControl.Items)
            {
                var container = MainWindow.Instance.NodesControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (container == null) continue;

                FrameworkElement nodeView = FindVisualParent<Node_NodeView>(portView) !;
                if (nodeView != null)
                {
                    if (nodeView?.DataContext is Node_NodeViewModel nodeViewModel && nodeViewModel == container.DataContext)
                    {
                        return (T)nodeView;
                    }
                }

            }

            //foreach (var item in MainWindow.Instance.NodesControl_ImageImport.Items)
            //{
            //    var container = MainWindow.Instance.NodesControl_ImageImport.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

            //    if (container == null) continue;

            //    var nodeView = FindVisualParent<ImageImport_NodeView>(portView);
            //    if (nodeView != null)
            //    {
            //        if (nodeView?.DataContext is ImageImport_NodeView nodeViewModel && nodeViewModel == container.DataContext)
            //        {
            //            return (T)container;
            //        }
            //    }
            //}

            return null;
        }

        public static TView FindView<TView>(object viewModel) where TView : FrameworkElement
        {
            foreach (Window window in Application.Current.Windows)
            {
                TView result = FindChild<TView>(window, viewModel);

                if (result != null) return result;
            }

            return null;
        }

        private static T FindChild<T>(DependencyObject parent, object vm) where T : FrameworkElement
        {
            if (parent == null)
                return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T fe && fe.DataContext == vm)
                    return fe;

                T result = FindChild<T>(child, vm);

                if (result != null)
                    return result;
            }

            return null;
        }

        public static IEnumerable<FrameworkElement?> FindConnectionElement(PortModel port)
        {
            foreach (var item in MainWindow.Instance.ConnectionsControl.Items)
            {
                var connection = MainWindow.Instance.ConnectionsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                if (connection == null && connection.DataContext == null) continue;

                var connectionViewModel = connection.DataContext as Node_ConnectionViewModel;

                if (connectionViewModel.ConnectionModel.FromPort == port || connectionViewModel.ConnectionModel.ToPort == port)
                {
                    yield return connection;
                }
            }

        }

        public static PortModel? HitTestPort(Point pos)
        {
            PortModel port = null;

            VisualTreeHelper.HitTest(
                MainWindow.Instance.MainCanvas,
                null,
                result =>
                {
                    if (result.VisualHit is FrameworkElement fe && fe.Tag is PortModel pm)
                    {
                        port = pm;
                        return HitTestResultBehavior.Stop;
                    }
                    return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(pos));

            return port;
        }

        public static bool IsValidConnection(PortModel from, PortModel to)
        {
            if (from.Type == PortType.Output && to.Type == PortType.Input && from.Owner != to.Owner)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static FrameworkElement GetWindowParent(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}

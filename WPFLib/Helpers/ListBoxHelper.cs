using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFLib
{
    public static partial class ListBoxHelper
    {
        public static void FocusOn(this ListBox list, object item, string controlName)
        {
            list.ScrollIntoView(item);
            list.SelectedValue = item;
            var o = list.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (o != null)
            {
                o.InvalidateVisual();
                o.Measure(new Size());
                var fe = VisualHelper.FindVisualChild<FrameworkElement>(o, controlName);
                if (fe != null)
                    fe.Focus();
                else
                    list.Focus();
            }
        }

        public static void FocusOn(this ListBox list, object item)
        {
            Action a = () => { list.ScrollIntoView(item); };
            Dispatcher.CurrentDispatcher.BeginInvoke(a, DispatcherPriority.Background);
            //list.ScrollIntoView(item);
            list.SelectedValue = item;
            list.Focus();
        }
    }
}

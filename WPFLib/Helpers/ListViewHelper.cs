using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFLib
{
    public static class ListViewHelper
    {
        public static void FocusOn(this ListView list, object item, string controlName)
        {
            list.ScrollIntoView(item);
            list.SelectedValue = item;
            var o = list.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
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

        public static void FocusOn(this ListView list, object item)
        {
            list.ScrollIntoView(item);
            list.SelectedValue = item;
            list.Focus();
        }
    }
}

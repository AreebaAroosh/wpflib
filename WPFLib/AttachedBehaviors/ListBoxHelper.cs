using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFLib
{
    public static partial class ListBoxHelper
    {
        public static readonly DependencyPropertyKey UnsubscribeFromFocysPropertyKey = DependencyProperty.RegisterAttachedReadOnly("UnsubscribeFromFocys", typeof(IDisposable), typeof(ListBoxHelper), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty UnsubscribeFromFocysProperty = UnsubscribeFromFocysPropertyKey.DependencyProperty;

        private static void SetUnsubscribeFromFocys(DependencyObject obj, IDisposable value)
        {
            obj.SetValue(UnsubscribeFromFocysPropertyKey, value);
        }

        public static IDisposable GetUnsubscribeFromFocys(DependencyObject obj)
        {
            return (IDisposable)obj.GetValue(UnsubscribeFromFocysProperty);
        }

        public static readonly DependencyProperty SelectItemWhenFocusWithInProperty = DependencyProperty.RegisterAttached("SelectItemWhenFocusWithIn", typeof(bool), typeof(ListBoxHelper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnSelectItemWhenFocusWithInChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceSelectItemWhenFocusWithIn) });

        public static void SetSelectItemWhenFocusWithIn(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectItemWhenFocusWithInProperty, value);
        }

        public static bool GetSelectItemWhenFocusWithIn(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectItemWhenFocusWithInProperty);
        }

        #region OnSelectItemWhenFocusWithInChanged
        private static void OnSelectItemWhenFocusWithInChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool)e.NewValue;
            var list = o as ItemsControl;
            if (val)
            {
                var uns = list.PreviewGotKeyboardFocusObservable().Subscribe((ev) =>
                {
                    var item = VisualHelper.GetAncestorByType(ev.EventArgs.NewFocus as DependencyObject, typeof(ListBoxItem)) as ListBoxItem;
                    if (item != null)
                    {
                        var evList = ev.Sender as ListBox;
                        if (evList != null)
                        {
                            evList.SelectedItem = item.Content;
                        }
                    }
                });
                list.SetValue(UnsubscribeFromFocysPropertyKey, uns);
            }
            else
            {
                var uns = list.GetValue(UnsubscribeFromFocysProperty) as IDisposable;
                if (uns != null)
                {
                    uns.Dispose();
                }
                list.ClearValue(UnsubscribeFromFocysPropertyKey);
            }
        }
        #endregion

        #region OnCoerceSelectItemWhenFocusWithIn
        private static object OnCoerceSelectItemWhenFocusWithIn(DependencyObject o, object value)
        {
            return value;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFLib.DataWrapper;

namespace WPFLib
{
    public class ValidationWrapper
    {
        public static readonly DependencyPropertyKey ManagerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Manager", typeof(object), typeof(ValidationWrapper), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ManagerProperty = ManagerPropertyKey.DependencyProperty;

        private static void SetManager(DependencyObject obj, object value)
        {
            obj.SetValue(ManagerPropertyKey, value);
        }

        public static object GetManager(DependencyObject obj)
        {
            return (object)obj.GetValue(ManagerProperty);
        }

        public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached("Id", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceId) });

        public static void SetId(DependencyObject obj, string value)
        {
            obj.SetValue(IdProperty, value);
        }

        public static string GetId(DependencyObject obj)
        {
            return (string)obj.GetValue(IdProperty);
        }

        #region OnIdChanged
        private static void OnIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var m = new ValidationWrapperAttachedManager((string)e.NewValue, (FrameworkElement)o);
            SetManager(o, m);
        }
        #endregion

        #region OnCoerceId
        private static object OnCoerceId(DependencyObject o, object value)
        {
            return value;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace WPFLib
{
    public static class ComboBoxFix
    {
        public static readonly DependencyProperty FixDataContextChangeProperty = DependencyProperty.RegisterAttached("FixDataContextChange", typeof(bool), typeof(ComboBoxFix), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnFixDataContextChangeChanged) });

        public static void SetFixDataContextChange(DependencyObject obj, bool value)
        {
            obj.SetValue(FixDataContextChangeProperty, value);
        }

        public static bool GetFixDataContextChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(FixDataContextChangeProperty);
        }

        private static void OnFixDataContextChangeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (WPFHelper.IsInDesignMode)
                return;
            var comboBox = o as ComboBox;
            if (comboBox != null)
            {
                comboBox.DataContextChanged += new DependencyPropertyChangedEventHandler(comboBox_DataContextChanged);
            }
        }

        static void comboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            var selectedValueBinding = BindingOperations.GetBinding(comboBox, ComboBox.SelectedValueProperty);
            var itemsSourceBinding = BindingOperations.GetBinding(comboBox, ComboBox.ItemsSourceProperty);
            if (selectedValueBinding == null || itemsSourceBinding == null)
                return;

            var b = BindingOperations.GetBinding(comboBox, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(comboBox, ComboBox.SelectedValueProperty);
            comboBox.Dispatcher.BeginInvoke((Action)(() => { comboBox.SetBinding(ComboBox.SelectedValueProperty, b.Clone()); }));
        }
    }
}

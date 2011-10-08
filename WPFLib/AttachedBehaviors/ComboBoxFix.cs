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
            // Решаем проблему не атомарности изменения SelectedItem и ItemsSource
            // По крайней мере может сначала поменяться ItemsSource ->
            // при этом SelectedItem может перейти в null и записаться в DataContext

            var comboBox = sender as ComboBox;

            var selectedValueBinding = BindingOperations.GetBinding(comboBox, ComboBox.SelectedValueProperty);
            var itemsSourceBinding = BindingOperations.GetBinding(comboBox, ComboBox.ItemsSourceProperty);
            if (selectedValueBinding == null || itemsSourceBinding == null)
            {
                // Если ItemsSource и SelectedItem одновременно не имеют байндинга - нам делать нечего
                return;
            }

            // Получаем байндинг который сейчас стоит на SelectedValue
            var b = BindingOperations.GetBinding(comboBox, ComboBox.SelectedValueProperty);

            // Очищаем байндинг на SelectedValue, для предотвращения неверного значения в DataContext
            BindingOperations.ClearBinding(comboBox, ComboBox.SelectedValueProperty);
            // Запускаем отложенную установку нового байндинга
            comboBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                // ItemsSource уже изменился, нам остается вернуть байндинг

                var newBinding = BindingOperations.GetBinding(comboBox, ComboBox.SelectedValueProperty);
                if (newBinding == null)
                {
                    // Если SelectedValue никто не трогал, установим ему байндинг как был раньше
                    comboBox.SetBinding(ComboBox.SelectedValueProperty, b.Clone());
                }
                else
                {
                    // Если же кто-то после нашей очистки установил новый байндинг(дата враппер например)
                    // Переустановим его, для того что бы SelectedValue правильно отобразился
                    // иначе он может быть пустым если ItemsSource был изменен после изменения SelectedValue
                    comboBox.SetBinding(ComboBox.SelectedValueProperty, newBinding.Clone());
                }
            }));
        }
    }
}

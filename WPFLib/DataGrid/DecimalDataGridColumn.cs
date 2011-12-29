using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Ksema.ObjectShell.WPF;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows;

namespace WPFLib
{
    public class DecimalDataGridColumn : DataGridTextColumn
    {
        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var tb = (TextBox)base.GenerateEditingElement(cell, dataItem);

            var b = this.Binding as Binding;
            b = b.Clone();
            //b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            //PresentationTraceSources.SetTraceLevel(b, PresentationTraceLevel.High);
            b.Converter = new DecimalMaskConverter();

            TextBoxMaskBehavior.SetMask(tb, NumericType.Decimal);
            var expr = tb.SetBinding(TextBox.TextProperty, b);
            return tb;
        }

        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            var textBox = editingElement as TextBox;
            textBox.Focus();
            var text = textBox.Text;
            TextCompositionEventArgs args = editingEventArgs as TextCompositionEventArgs;
            if (args != null)
            {
                string str2 = args.Text;
                if (TextBoxMaskBehavior.IsSymbolValid(TextBoxMaskBehavior.GetMask(textBox), str2))
                {
                    textBox.Text = str2;
                    textBox.Select(str2.Length, 0);
                }
                return text;
            }
            if (!(editingEventArgs is MouseButtonEventArgs) || !PlaceCaretOnTextBox(textBox, Mouse.GetPosition(textBox)))
            {
                textBox.SelectAll();
            }
            return text;
        }

        private static bool PlaceCaretOnTextBox(TextBox textBox, Point position)
        {
            int characterIndexFromPoint = textBox.GetCharacterIndexFromPoint(position, false);
            if (characterIndexFromPoint >= 0)
            {
                textBox.Select(characterIndexFromPoint, 0);
                return true;
            }
            return false;
        }
    }
}

/* Copyright(c) 2009, Rubenhak
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   -> Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   -> Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Input;
using System.Linq;

/*
 * This file is the part of Rubenhak.Common.WPF library.
 * New versions are be found at http://rubenhak.com/.
 */
namespace WPFLib
{
    #region Documentation Tags
    /// <summary>
    ///     WPF Maskable TextBox class. Just specify the TextBoxMaskBehavior.Mask attached property to a TextBox. 
    ///     It protect your TextBox from unwanted non numeric symbols and make it easy to modify your numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">February 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
    ///     </list>
    /// </para>
    /// </remarks>
    #endregion
    public class TextBoxMaskBehavior
    {
        public static readonly DependencyProperty IsBindingCheckedProperty = DependencyProperty.RegisterAttached("IsBindingChecked", typeof(bool), typeof(TextBoxMaskBehavior), new FrameworkPropertyMetadata());

        public static void SetIsBindingChecked(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBindingCheckedProperty, value);
        }

        public static bool GetIsBindingChecked(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBindingCheckedProperty);
        }

        #region MinimumValue Property

        public static double GetMinimumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MinimumValueProperty);
        }

        public static void SetMinimumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MinimumValueProperty, value);
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.RegisterAttached(
                "MinimumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MinimumValueChangedCallback)
                );

        private static void MinimumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region MaximumValue Property

        public static double GetMaximumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaximumValueProperty);
        }

        public static void SetMaximumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaximumValueProperty, value);
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.RegisterAttached(
                "MaximumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MaximumValueChangedCallback)
                );

        private static void MaximumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region Mask Property

        public static NumericType GetMask(DependencyObject obj)
        {
            return (NumericType)obj.GetValue(MaskProperty);
        }

        public static void SetMask(DependencyObject obj, NumericType value)
        {
            obj.SetValue(MaskProperty, value);
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached(
                "Mask",
                typeof(NumericType),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(MaskChangedCallback)
                );

        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox)
            {
                (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;
                (e.OldValue as TextBox).PreviewKeyDown -= TextBox_PreviewKeyDown;
                DataObject.RemovePastingHandler((e.OldValue as TextBox), (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            //Observable.Return(_this.IsLoaded)
            //    .Merge(_this.LoadedObservable()
            //                .Select(ev => true)
            //        )
            //    .Where(v => v)
            //    .Take(1)
            //    .Subscribe(() => CheckTextBox(_this));

            if ((NumericType)e.NewValue != NumericType.Any)
            {
                _this.TextChanged += new TextChangedEventHandler(_this_TextChanged);
                _this.PreviewTextInput += TextBox_PreviewTextInput;
                _this.PreviewKeyDown += TextBox_PreviewKeyDown;
                DataObject.AddPastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            ValidateTextBox(_this);
        }

        static void CheckTextBox(TextBox tb)
        {
            var mask = GetMask(tb);
            if (mask == NumericType.Decimal)
            {
                var bind = tb.GetBindingExpression(TextBox.TextProperty);
                if (!(bind.ParentBinding.Converter is DecimalMaskConverter))
                {
                    throw new InvalidOperationException("For Decimal mask Converter must be DecimalMaskConverter");
                }
            }

        }

        static void _this_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        static void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Делаем обработку Del и BackSpace, тк они не вызывают TextInput
            // значение идет сразу дальше
            // Потом проверяем строку на равенство "-"
            TextBox _this = (sender as TextBox);
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                string text = _this.Text;
                var selectionLength = _this.SelectionLength;
                if (!String.IsNullOrEmpty(text))
                {
                    int caret = _this.CaretIndex;
                    if (_this.SelectionLength > 0)
                    {
                        var selected = text.Substring(_this.SelectionStart, _this.SelectionLength);
                        text = text.Substring(0, _this.SelectionStart) + _this.Text.Substring(_this.SelectionStart + _this.SelectionLength);
                        if (e.Key == Key.Back && text == NumberFormatInfo.CurrentInfo.NegativeSign && selected == "0")
                        {
                            text = "0";
                            caret = 0;
                            selectionLength = 1;
                        }
                        else
                        {
                            caret = _this.SelectionStart;
                        }
                    }
                    else
                    {
                        if (e.Key == Key.Delete && caret < text.Length)
                        {
                            text = _this.Text.Substring(0, caret) + _this.Text.Substring(caret + 1);
                        }
                        else if (e.Key == Key.Back && caret > 0)
                        {
                            text = _this.Text.Substring(0, caret - 1) + _this.Text.Substring(caret);
                            caret--;
                        }
                    }
                    if (text == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + "0";
                        caret = 1;
                        selectionLength = 1;
                    }
                    if (String.IsNullOrEmpty(text))
                    {
                        text = "0";
                        caret = 0;
                        selectionLength = 1;
                    }
                    _this.Text = text;
                    _this.CaretIndex = caret;
                    _this.SelectionLength = selectionLength;
                }
                e.Handled = true;
            }
        }

        #endregion

        #region Private Static Methods

        private static void ValidateTextBox(TextBox _this)
        {
            if (GetMask(_this) != NumericType.Any)
            {
                bool validated;
                var validatedValue = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this), out validated);
                if (validated)
                    _this.Text = validatedValue;
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            if (_this.IsReadOnly)
                return;
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            bool validated;
            clipboard = ValidateValue(GetMask(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this), out validated);
            if (!string.IsNullOrEmpty(clipboard))
            {
                _this.Text = clipboard;
            }
            if (validated)
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            if (_this.IsReadOnly)
                return;
            var eText = e.Text;
            if (eText == NumberFormatInfo.CurrentInfo.PositiveSign && _this.Text.StartsWith(NumberFormatInfo.CurrentInfo.NegativeSign))
            {
                eText = NumberFormatInfo.CurrentInfo.NegativeSign;
            }
            bool isValid = IsSymbolValid(GetMask(_this), eText);
            e.Handled = !isValid;
            if (isValid)
            {
                if (!GetIsBindingChecked(_this))
                {
                    CheckTextBox(_this);
                    SetIsBindingChecked(_this, true);
                }
                if (_this.Text.StartsWith(NumberFormatInfo.CurrentInfo.NegativeSign) && _this.CaretIndex < NumberFormatInfo.CurrentInfo.NegativeSign.Length && _this.SelectionLength == 0)
                {
                    e.Handled = true;
                    return;
                }
                int caret = _this.CaretIndex;
                string text = _this.Text;
                bool textInserted = false;
                int selectionLength = 0;

                if (_this.SelectionLength > 0)
                {
                    text = text.Substring(0, _this.SelectionStart) +
                            text.Substring(_this.SelectionStart + _this.SelectionLength);
                    caret = _this.SelectionStart;
                }

                if (eText == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    while (true)
                    {
                        int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        if (ind == -1)
                            break;

                        text = text.Substring(0, ind) + text.Substring(ind + 1);
                        if (caret > ind)
                            caret--;
                    }

                    if (caret == 0)
                    {
                        text = "0" + text;
                        caret++;
                    }
                    else
                    {
                        if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                        {
                            text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                            caret++;
                        }
                    }

                    if (caret == text.Length)
                    {
                        selectionLength = 1;
                        textInserted = true;
                        text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0";
                        caret++;
                    }
                }
                else if (eText == NumberFormatInfo.CurrentInfo.NegativeSign && selectionLength < text.Length)
                {
                    textInserted = true;
                    if (_this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                    {
                        text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                        if (caret != 0)
                            caret--;
                    }
                    else
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text;
                        caret++;
                    }
                }

                if (!textInserted)
                {
                    text = text.Substring(0, caret) + eText +
                        ((caret < _this.Text.Length) ? text.Substring(caret) : string.Empty);

                    caret++;
                }

                try
                {
                    double val = 0;
                    if (text == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + "0";
                        caret = 1;
                        selectionLength = 1;
                    }
                    if (!String.IsNullOrEmpty(text))
                    {
                        val = Convert.ToDouble(text);
                    }
                    double newVal = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val);
                    if (val != newVal)
                    {
                        text = newVal.ToString();
                    }
                    else if (val == 0)
                    {
                        //if (!text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        //    text = "0";
                    }
                }
                catch
                {
                    text = "0";
                }

                while (text.Length > 1 && text[0] == '0' && string.Empty + text[1] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    text = text.Substring(1);
                    if (caret > 0)
                        caret--;
                }

                while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign && text[1] == '0' && string.Empty + text[2] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                    if (caret > 1)
                        caret--;
                }

                if (caret > text.Length)
                    caret = text.Length;

                _this.Text = text;
                _this.CaretIndex = caret;
                _this.SelectionStart = caret;
                _this.SelectionLength = selectionLength;
                //_this.Text = text;
                e.Handled = true;
            }
        }

        private static string ValidateValue(NumericType mask, string value, double min, double max, out bool validated)
        {
            validated = false;
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            switch (mask)
            {
                case NumericType.Integer:
                    try
                    {
                        Int64 result;
                        if (Int64.TryParse(value, out result))
                        {
                            validated = true;
                            return value;
                        }
                    }
                    catch
                    {
                    }
                    return string.Empty;

                case NumericType.Decimal:
                    try
                    {
                        double result;
                        if (Double.TryParse(value, out result))
                        {
                            validated = true;
                            return value;
                        }
                    }
                    catch
                    {
                    }
                    return string.Empty;
            }

            return value;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }

            return value;
        }

        public static bool IsSymbolValid(NumericType mask, string str)
        {
            switch (mask)
            {
                case NumericType.Any:
                    return true;

                case NumericType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case NumericType.Decimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
            }

            if (mask.Equals(NumericType.Integer) || mask.Equals(NumericType.Decimal))
            {
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch))
                        return false;
                }

                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum NumericType
    {
        Any,
        Integer,
        Decimal
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;

namespace WPFLib
{
    public class DecimalTextBox : TextBox
    {
        private static string decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        /// <summary>  
        /// Get/Set the decimal value.  This is not a dependency property, so you cannot bind things to it.  
        /// To bind a decimal data value to this control, bind to the Text property, and do a conversion.  
        /// </summary>  
        public decimal DecimalValue
        {
            get
            {
                decimal value = 0.0M;
                if (!string.IsNullOrEmpty(this.Text))
                {
                    value = Convert.ToDecimal(this.Text);
                }
                return value;
            }
            set
            {
                if (value < 0 && this.NegativeBlank)
                {
                    base.Text = string.Empty;
                }
                else
                {
                    string decFormat = string.Format("{{0:{0}}}{{1}}", "0.".PadRight(this.MaxDecimalPlaces + 2, '#'));
                    base.Text = string.Format(decFormat, value);
                }
            }
        }
        #region Text Entry Validation

        /// <summary>  
        /// Prevent space characters.    
        /// Thanx to aelij on http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/5460722b-619b-4937-b939-38610e9e01ea  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        /// <summary>  
        /// Determines if text input is valid, and if not, rejects it.  
        /// Note that the space character doesn't result in a call to this method,  
        /// so it is handled separately in OnPreviewKeyDown().  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            // Force a binding update if the user presses enter  
            if (e.Text == "\r")
            {
                e.Handled = true;
                BindingExpression be = this.GetBindingExpression(TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
            else
            {
                e.Handled = !isMyTextValid(computeProposed(e.Text));
            }
        }

        private bool isMyTextValid(String text)
        {
            bool isValid = false;
            if (!AllowNegative && text.StartsWith("-"))
            {
                return false;
            }
            if (!String.IsNullOrEmpty(text == null ? null : text.Trim()))
            {
                if (text.Contains(decimalSeparator) && this.MaxDecimalPlaces <= 0)
                {
                    isValid = false;
                }
                else
                {
                    Decimal result = Decimal.MinValue;
                    if (Decimal.TryParse(text, out result))
                    {
                        if (result != Decimal.MinValue)
                        {
                            isValid = checkDecimalPlaces(result);
                        }
                    }
                }
            }

            return isValid;
        }

        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.Register("AllowNegative", typeof(bool), typeof(DecimalTextBox), new FrameworkPropertyMetadata() { DefaultValue = true });
        public bool AllowNegative
        {
            get { return (bool)GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }  

        /// <summary>  
        /// Check to see if the MaxDecimalPlaces rule is violated  
        /// </summary>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        private bool checkDecimalPlaces(decimal value)
        {
            string textValue = value.ToString("0.############");
            string[] parts = textValue.Split(new string[] { decimalSeparator }, StringSplitOptions.None);
            bool valid = (parts.Length <= 1) || (parts[1].Length <= this.MaxDecimalPlaces);
            return valid;
        }

        /// <summary>  
        /// Compute the proposed text - what would be in the textbox if the input was allowed.  
        /// </summary>  
        /// <param name="newText"></param>  
        /// <returns></returns>  
        private string computeProposed(string newText)
        {
            string text = base.Text;
            if (base.SelectionLength > 0)
            {
                text = text.Remove(this.SelectionStart, this.SelectionLength);
            }
            return text.Insert(this.SelectionStart, newText);
        }
        #endregion

        #region Special handling for pasting events

        static DecimalTextBox()
        {
            EventManager.RegisterClassHandler(
                typeof(DecimalTextBox),
                DataObject.PastingEvent,
                (DataObjectPastingEventHandler)((sender, e) =>
                {
                    if (!IsDataValid(e.DataObject, sender as DecimalTextBox))
                    {
                        DataObject data = new DataObject();
                        data.SetText(String.Empty);
                        e.DataObject = data;
                        e.Handled = false;
                    }
                }));

            TextProperty.OverrideMetadata(
                typeof(DecimalTextBox),
                new FrameworkPropertyMetadata(null,
                    (CoerceValueCallback)((DependencyObject element, Object baseValue) =>
                    {
                        return IsTextValid(baseValue.ToString()) ? baseValue : string.Empty;
                    })));
        }


        private static bool IsDataValid(IDataObject data, DecimalTextBox dtb)
        {
            bool isValid = false;
            if (data != null)
            {
                String text = data.GetData(DataFormats.Text) as String;
                if (dtb != null)
                {
                    isValid = dtb.isMyTextValid(dtb.computeProposed(text));
                }
                else
                {
                    isValid = IsTextValid(text);
                }
            }

            return isValid;
        }

        private static bool IsTextValid(String text)
        {
            bool isValid = false;
            if (!String.IsNullOrEmpty(text == null ? null : text.Trim()))
            {
                Decimal result = Decimal.MinValue;
                if (Decimal.TryParse(text, out result))
                {
                    if (result != Decimal.MinValue)
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }
        #endregion

        #region DependencyProperty - MaxDecimalPlaces
        /// <summary>  
        /// Dependency property to set the maximum number of decimal places allowed  
        /// </summary>  
        public int MaxDecimalPlaces
        {
            get { return (int)GetValue(MaxDecimalPlacesProperty); }
            set { SetValue(MaxDecimalPlacesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDecimalPlaces.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty MaxDecimalPlacesProperty =
            DependencyProperty.Register("MaxDecimalPlaces", typeof(int), typeof(DecimalTextBox), new UIPropertyMetadata(2));
        #endregion

        #region Dependency Property - NegativeBlank
        /// <summary>  
        /// Dependency property to show a blank in the textbox if the value is negative.  
        /// </summary>  
        public bool NegativeBlank
        {
            get { return (bool)GetValue(NegativeBlankProperty); }
            set { SetValue(NegativeBlankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NegativeBlank.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty NegativeBlankProperty =
            DependencyProperty.Register("NegativeBlank", typeof(bool), typeof(DecimalTextBox), new UIPropertyMetadata(true));
        #endregion

        #region Drag/Drop
        /// <summary>  
        /// Currently not allowing Drag/Drop.  May in the future add a dependency property called  
        /// 'AllowDrop', which when true, will cause this method to test and possibly accept a drop.  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnDrop(DragEventArgs e)
        {
            // no drag and drop allowed  
            e.Handled = true;
            base.OnDrop(e);
        }

        /// <summary>  
        /// See comments for OnDrop  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnDragOver(DragEventArgs e)
        {
            // no drag and drop allowed  
            e.Handled = true;
            e.Effects = DragDropEffects.None;
            base.OnDragOver(e);
        }
        #endregion
    }  
}

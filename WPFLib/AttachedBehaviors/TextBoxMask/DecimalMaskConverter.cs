using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace WPFLib
{
    public class DecimalMaskConverter : MarkupExtension, IValueConverter
    {
        private string _lastValue;
        bool negativeZero = false;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (negativeZero)
            {
                // negativeZero в false установит только ConvertBack,
                // тк байндинг просто повторно может дернуть Convert
                return _lastValue;
            }
            var valueString = value != null ? ToString(value) : String.Empty;
            if (_lastValue == valueString + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0")
                return _lastValue;
            return valueString;
        }

        string ToString(object value)
        {
            if (value is double)
            {
                return ((double)value).ToString(NumberFormatInfo.CurrentInfo);
            }
            else if (value is decimal)
            {
                return ((decimal)value).ToString(NumberFormatInfo.CurrentInfo);
            }
            throw new Exception("Unsupported type " + value.GetType().FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _lastValue = (string)value;

            // Тупое решение проблемы с конвертерами
            if (_lastValue == "-0")
            {
                negativeZero = true;
                return value;
            }
            else
            {
                negativeZero = false;
            }

            if (targetType == typeof(double))
            {
                if (String.IsNullOrWhiteSpace(_lastValue))
                    return 0d;
                var doubleVal = double.Parse(_lastValue);
                return doubleVal;
            }
            else if (targetType == typeof(decimal))
            {
                if (String.IsNullOrWhiteSpace(_lastValue))
                    return 0M;
                var val = decimal.Parse(_lastValue);
                return val;
            }
            throw new Exception("Unsupported type " + targetType.FullName);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

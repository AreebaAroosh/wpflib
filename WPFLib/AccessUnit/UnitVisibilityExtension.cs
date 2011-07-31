using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFLib.AccessUnit;
using System.Windows.Data;

namespace WPFLib
{
    public class AccessUnitToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mode = (AccessUnitMode)value;
            switch (mode)
            {
                case AccessUnitMode.Edit:
                case AccessUnitMode.ReadOnly:
                    return Visibility.Visible;
                case AccessUnitMode.Hidden:
                    return Visibility.Collapsed;
                default:
                    throw new Exception();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UnitVisibilityExtension : AccessUnitBaseExtension
    {
        static AccessUnitToVisibilityConverter Converter = new AccessUnitToVisibilityConverter();
     
        public UnitVisibilityExtension(string field)
            : base(field)
        {
        }

        protected override IValueConverter ProvideConverter()
        {
            return Converter;
        }
    }
}

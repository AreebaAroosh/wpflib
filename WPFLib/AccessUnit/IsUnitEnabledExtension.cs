using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.AccessUnit;
using System.Windows.Data;

namespace WPFLib
{
    public class AccessUnitToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mode = (AccessUnitMode)value;
            switch (mode)
            {
                case AccessUnitMode.Edit:
                    return true;
                case AccessUnitMode.ReadOnly:
                    return false;
                case AccessUnitMode.Hidden:
                    return false;
                default:
                    throw new Exception();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsUnitEnabledExtension : AccessUnitBaseExtension
    {
        static AccessUnitToEnabledConverter Converter = new AccessUnitToEnabledConverter();

        public IsUnitEnabledExtension(string field)
            : base(field)
        {
        }

        protected override IValueConverter ProvideConverter()
        {
            return Converter;
        }
    }
}

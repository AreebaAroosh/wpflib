using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.AccessUnit;
using System.Windows.Data;

namespace WPFLib
{
    public class AccessUnitToIsReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mode = (AccessUnitMode)value;
            switch (mode)
            {
                case AccessUnitMode.Edit:
                    return false;
                case AccessUnitMode.ReadOnly:
                    return true;
                case AccessUnitMode.Hidden:
                    return true;
                default:
                    throw new Exception();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsUnitReadOnlyExtension : AccessUnitBaseExtension
    {
        static AccessUnitToIsReadOnlyConverter Converter = new AccessUnitToIsReadOnlyConverter();

        public IsUnitReadOnlyExtension(string field)
            : base(field)
        {
        }

        protected override IValueConverter ProvideConverter()
        {
            return Converter;
        }
    }
}

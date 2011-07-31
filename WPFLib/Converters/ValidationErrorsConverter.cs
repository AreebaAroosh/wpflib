using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace WPFLib
{
    public class ValidationErrorsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length > 0)
            {
                var value = values[0];
                var coll = value as ReadOnlyObservableCollection<ValidationError>;
                if (coll != null && coll.Count > 0)
                {
                    if (coll.Count == 1)
                    {
                        return coll[0].ErrorContent;
                    }
                    var r = (from error in coll
                             select error.ErrorContent.ToString()).ToArray();
                    return String.Join(Environment.NewLine, r);
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationErrorsConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var coll = value as ReadOnlyObservableCollection<ValidationError>;
            if (coll != null && coll.Count > 0)
            {
                if (coll.Count == 1)
                {
                    return coll[0].ErrorContent;
                }
                var r = (from error in coll
                         select error.ErrorContent.ToString()).ToArray();
                return String.Join(Environment.NewLine, r);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

	public class ValidationErrorsConverterDisabled : ValidationErrorsConverter
	{
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (base.Convert(value, targetType, parameter, culture) + Environment.NewLine + "Для изменения возпользуйтесь контекстным меню").Trim();
		}
	}
}

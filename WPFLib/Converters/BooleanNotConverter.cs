using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace WPFLib
{
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }

	public class BooleanToVisibilityNotConverter : IValueConverter
	{
		private BooleanToVisibilityConverter converter = new BooleanToVisibilityConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return converter.Convert(!(bool)value, targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return converter.ConvertBack(!(bool)value, targetType, parameter, culture);
		}
	}

	public class InvisibleIfDisabled
	{
		public static bool GetIsInUse(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsInUseProperty);
		}

		public static void SetIsInUse(DependencyObject obj, bool value)
		{
			obj.SetValue(IsInUseProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsInUse.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsInUseProperty =
			DependencyProperty.RegisterAttached("IsInUse", typeof(bool), typeof(InvisibleIfDisabled), new UIPropertyMetadata(false, IsInUseChanged));

		private static void IsInUseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Control ctrl = d as Control;
			if (ctrl!=null && (bool)e.NewValue)
			{
				var binding = new Binding("IsEnabled") {Converter = new BooleanToVisibilityConverter(), RelativeSource = new RelativeSource(RelativeSourceMode.Self)};
				ctrl.SetBinding(Control.VisibilityProperty, binding);
			}
		}
	}
}

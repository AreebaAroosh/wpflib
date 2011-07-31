using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFLib
{
    public static class DialogHelper
    {
        public static bool? ShowOwnedDialog(this Window wnd)
        {
			try
			{
				if (Application.Current != null && Application.Current.MainWindow != null && Application.Current.MainWindow != wnd)
					wnd.Owner = Application.Current.MainWindow;
			}
			catch (Exception error)
			{
			}
            return wnd.ShowDialog();
        }

		public static bool RealValue(this bool? value)
		{
			return value.HasValue ? value.Value : false;
		}
    }
}

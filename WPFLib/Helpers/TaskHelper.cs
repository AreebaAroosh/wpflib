using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFLib.Contracts;
using System.Threading.Tasks;

namespace System
{
    public static class TaskHelper
    {
        public static Task<T> AsTask<T>(this T o)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(o);
            return tcs.Task;
        }

        public static void ErrorMessage(this object o, Task e)
        {
            ErrorMessage(o, e.Exception);
        }

        public static void ErrorMessage(this object o, Exception e)
        {
            var service = o.Resolve<IDialogService>();
			if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess()/* Application.Current.MainWindow != null*/)
            {
                Application.Current.Dispatcher.BeginInvoke(() => {
                    service.ErrorMessage(e);
                });
            }
            else
            {
                service.ErrorMessage(e);
            }
        }
    }
}

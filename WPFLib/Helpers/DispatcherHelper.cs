using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace System
{
    public static class DispatcherHelper
    {
        public static void BeginInvoke(this Dispatcher dipatcher, Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            dipatcher.BeginInvoke(action, priority);
        }

        public static void Invoke(this Dispatcher dipatcher, Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            dipatcher.Invoke(action, priority);
        }

        private static object exitFrames(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        /// <summary>
        /// Аналог WinForms DoEvents, в частности начинает процессинг байндинга
        /// </summary>
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(exitFrames), frame);
            Dispatcher.PushFrame(frame);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    public interface INavigationSuspendStateChanges : IDisposable
    {
        /// <summary>
        /// Учитывать ли последнее изменение произошедшее при запрете нотификаций
        /// </summary>
        bool GetLastChange
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    public class NavigationSuspendStateChanges : INavigationSuspendStateChanges
    {
        NavigationManager manager;

        internal NavigationSuspendStateChanges(NavigationManager _manager, bool getLastChange)
        {
            manager = _manager;
            GetLastChange = getLastChange;
        }

        #region INavigationSuspendStateChanges Members

        public bool GetLastChange
        {
            get;
            private set;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            manager.ResumeStateChanges(this);
        }

        #endregion
    }
}

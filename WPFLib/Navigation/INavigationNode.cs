using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    public interface INavigationNode
    {
        /// <summary>
        /// Восстановить состояние узла
        /// </summary>
        /// <param name="state"></param>
        void RestoreState(NavigationState state);

        /// <summary>
        /// Локальное состояние узла
        /// </summary>
        NavigationNodeState[] LocalState
        {
            get;
        }
    }
}

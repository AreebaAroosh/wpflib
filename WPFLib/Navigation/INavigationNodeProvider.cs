using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    public interface INavigationNodeProvider
    {
        bool Open(NavigationState state);
    }
}

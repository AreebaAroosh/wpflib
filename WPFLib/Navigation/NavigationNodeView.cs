using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Collections;

namespace WPFLib.Navigation
{
    public class NavigationNodeView
    {
        WeakDictionary<INavigationNode, object> dict = new WeakDictionary<INavigationNode, object>();

        public void AddChild(INavigationNode node)
        {
            dict[node] = dict;
        }

        public List<INavigationNode> Children
        {
            get
            {
                dict.RemoveCollectedEntries();
                return dict.Keys.ToList();
            }
        }
    }
}

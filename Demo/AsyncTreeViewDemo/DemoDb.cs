using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace AsyncTreeViewDemo
{
    public class DemoDb
    {
        static ObservableCollection<DemoItem> _all;

        static IEnumerable<DemoItem> Generate()
        {
            Func<IEnumerable<string>, IEnumerable<string>> addLevel = (source) =>
            {
                return source.SelectMany(i => Enumerable.Range(1, 5).Select(j => i + "-" + j));
            };


            IEnumerable<string> allLabels = Enumerable.Range(1, 5).Select(v => v.ToString());
            IEnumerable<string> prev = allLabels;
            for (int i = 0; i < 4; i++)
            {
                prev = addLevel(prev);
                allLabels = allLabels.Concat(prev);
            }

            return allLabels.Select(l => new DemoItem("Item-" + l)).ToList();
        }

        public static IQueryable<DemoItem> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new ObservableCollection<DemoItem>(Generate());
                }
                return _all.AsQueryable();
            }
        }

        public static IEnumerable<DemoItem> LoadChildren(DemoItem item)
        {
            Regex IsChild = new Regex(String.Format(@"^{0}-\d+$", item.Label));
            var r = All.Where(child => IsChild.IsMatch(child.Label));
            foreach (var child in r)
            {
                Thread.Sleep(10);
                child.Parent = item;
            }
            return r;
        }

        public static IEnumerable<DemoItem> LoadRoot()
        {
            Regex IsChild = new Regex(@"^Item-\d+$");
            var r = All.Where(child => IsChild.IsMatch(child.Label));
            return r;
        }

        public static DemoItem LoadParent(DemoItem item)
        {
            var parent = All.Where(i => i.Label == item.Label.Substring(0, item.Label.Length - 2)).FirstOrDefault();
            return parent;
        }
    }
}

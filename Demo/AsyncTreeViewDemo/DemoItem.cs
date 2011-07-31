using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Misc;
using System.ComponentModel;
using WPFLib.Contracts;
using System.Collections.ObjectModel;
using WPFLib;

namespace AsyncTreeViewDemo
{
    public class DemoItem : DispatcherPropertyChangedHelper, IAsyncTreeItem
    {
        public DemoItem(string label)
        {
            _Label = label;
        }
        #region LabelProperty
        public static readonly PropertyChangedEventArgs LabelArgs = PropertyChangedHelper.CreateArgs<DemoItem>(c => c.Label);
        private string _Label;

        public string Label
        {
            get
            {
                return _Label;
            }
            set
            {
                var oldValue = Label;
                _Label = value;
                if (oldValue != value)
                {
                    OnLabelChanged(oldValue, value);
                    OnPropertyChanged(LabelArgs);
                }
            }
        }

        protected virtual void OnLabelChanged(string oldValue, string newValue)
        {
        }
        #endregion


        public bool IsLoaded
        {
            get;
            set;
        }

        #region ChildrenProperty
        public static readonly PropertyChangedEventArgs ChildrenArgs = PropertyChangedHelper.CreateArgs<DemoItem>(c => c.Children);
        private DispatcherCollection<DemoItem> _Children;

        public DispatcherCollection<DemoItem> Children
        {
            get
            {
                return _Children;
            }
            set
            {
                var oldValue = Children;
                _Children = value;
                if (oldValue != value)
                {
                    OnChildrenChanged(oldValue, value);
                    OnPropertyChanged(ChildrenArgs);
                }
            }
        }

        protected virtual void OnChildrenChanged(DispatcherCollection<DemoItem> oldValue, DispatcherCollection<DemoItem> newValue)
        {
        }
        #endregion

        #region ParentProperty
        public static readonly PropertyChangedEventArgs ParentArgs = PropertyChangedHelper.CreateArgs<DemoItem>(c => c.Parent);
        private DemoItem _Parent;

        public DemoItem Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                var oldValue = Parent;
                _Parent = value;
                if (oldValue != value)
                {
                    OnParentChanged(oldValue, value);
                    OnPropertyChanged(ParentArgs);
                }
            }
        }

        protected virtual void OnParentChanged(DemoItem oldValue, DemoItem newValue)
        {
        }
        #endregion

        #region CanChangeSelectionProperty
        public static readonly PropertyChangedEventArgs CanChangeSelectionArgs = PropertyChangedHelper.CreateArgs<DemoItem>(c => c.CanChangeSelection);
        private bool _CanChangeSelection = true;

        public bool CanChangeSelection
        {
            get
            {
                return _CanChangeSelection;
            }
            set
            {
                var oldValue = CanChangeSelection;
                _CanChangeSelection = value;
                if (oldValue != value)
                {
                    OnCanChangeSelectionChanged(oldValue, value);
                    OnPropertyChanged(CanChangeSelectionArgs);
                }
            }
        }

        protected virtual void OnCanChangeSelectionChanged(bool oldValue, bool newValue)
        {
        }
        #endregion

        public override string ToString()
        {
            return Label;
        }
    }
}

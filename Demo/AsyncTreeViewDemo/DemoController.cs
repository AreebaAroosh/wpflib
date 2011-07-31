using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Misc;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WPFLib.Contracts;
using System.Threading.Tasks;
using System.Threading;
using WPFLib;

namespace AsyncTreeViewDemo
{
    public class DemoController : DispatcherPropertyChangedHelper, IAsyncTreeController, IAsyncTreeItemHandler<DemoItem>, IAsyncSearchListBoxController
    {
        public enum StateEnum
        {
            Work,
            Ready
        }

        #region StateProperty
        public static readonly PropertyChangedEventArgs StateArgs = PropertyChangedHelper.CreateArgs<DemoController>(c => c.State);
        private StateEnum _State;

        public StateEnum State
        {
            get
            {
                return _State;
            }
            private set
            {
                var oldValue = State;
                _State = value;
                if (oldValue != value)
                {
                    OnStateChanged(oldValue, value);
                    OnPropertyChanged(StateArgs);
                }
            }
        }

        protected virtual void OnStateChanged(StateEnum oldValue, StateEnum newValue)
        {
        }
        #endregion


        public DemoController()
        {
            State = StateEnum.Work;
            Task.Factory.StartNew(() => Root = new DispatcherCollection<DemoItem>(DemoDb.LoadRoot())).ContinueWith((t) => State = StateEnum.Ready);
        }

        #region CurrentItemProperty
        public static readonly PropertyChangedEventArgs CurrentItemArgs = PropertyChangedHelper.CreateArgs<DemoController>(c => c.CurrentItem);
        private DemoItem _CurrentItem;

        public DemoItem CurrentItem
        {
            get
            {
                return _CurrentItem;
            }
            set
            {
                var oldValue = CurrentItem;
                _CurrentItem = value;
                if (oldValue != value)
                {
                    OnCurrentItemChanged(oldValue, value);
                    OnPropertyChanged(CurrentItemArgs);
                }
            }
        }

        protected virtual void OnCurrentItemChanged(DemoItem oldValue, DemoItem newValue)
        {
            IsSelection = newValue != null;
        }
        #endregion

        #region IsSelectionProperty
        public static readonly PropertyChangedEventArgs IsSelectionArgs = PropertyChangedHelper.CreateArgs<DemoController>(c => c.IsSelection);
        private bool _IsSelection;

        public bool IsSelection
        {
            get
            {
                return _IsSelection;
            }
            private set
            {
                var oldValue = IsSelection;
                _IsSelection = value;
                if (oldValue != value)
                {
                    OnIsSelectionChanged(oldValue, value);
                    OnPropertyChanged(IsSelectionArgs);
                }
            }
        }

        protected virtual void OnIsSelectionChanged(bool oldValue, bool newValue)
        {
        }
        #endregion


        #region RootProperty
        public static readonly PropertyChangedEventArgs RootArgs = PropertyChangedHelper.CreateArgs<DemoController>(c => c.Root);
        private DispatcherCollection<DemoItem> _Root;

        public DispatcherCollection<DemoItem> Root
        {
            get
            {
                return _Root;
            }
            set
            {
                var oldValue = Root;
                _Root = value;
                if (oldValue != value)
                {
                    OnRootChanged(oldValue, value);
                    OnPropertyChanged(RootArgs);
                }
            }
        }

        protected virtual void OnRootChanged(DispatcherCollection<DemoItem> oldValue, DispatcherCollection<DemoItem> newValue)
        {
        }
        #endregion

        public System.Threading.Tasks.Task<bool> CanChangeSelection()
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                if (CurrentItem == null)
                    return true;
                Thread.Sleep(100);
                return CurrentItem.CanChangeSelection;
            });
        }

        public System.Threading.Tasks.Task LoadTreeItem(DemoItem entity)
        {
            return Task.Factory.StartNew(() =>
                {
                    // Загружаем информацию для узла
                    // это его дети
                    entity.Children = new DispatcherCollection<DemoItem>(DemoDb.LoadChildren(entity));
                    // и его родитель
                    entity.Parent = DemoDb.LoadParent(entity);
                });
        }

        public IAsyncTreeItem GetParent(DemoItem entity)
        {
            if (entity.Parent == null)
            {
                // Если это корневой элемент, то возвращаем заглушку
                // которая может вернуть индекс элемента в корне
                return AsyncTreeView.CreateRootItem((e) => { return Root.IndexOf((DemoItem)e); });
            }
            else
            {
                return entity.Parent;
            }
        }

        public int IndexOf(DemoItem entity, IAsyncTreeItem child)
        {
            // Просто вернем индекс элемента
            return entity.Children.IndexOf((DemoItem)child);
        }

        public Task<IEnumerable<object>> GetResults(CancellationTokenSource token, string searchQuery, int maxResults)
        {
            return Task<IEnumerable<object>>.Factory.StartNew(() =>
                {
                    // Найдем все что подходит
                    var r = DemoDb.All.Where(i => i.Label.ToLower().Contains(searchQuery.ToLower())).Take(maxResults).ToList();
                    // И проинициализируем родителя
                    foreach (var item in r)
                    {
                        item.Parent = DemoDb.LoadParent(item);
                    }
                    return r;
                });
        }
    }
}

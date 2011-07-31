using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using WPFLib.Contracts;
using WPFLib.Misc;
using System.Collections.ObjectModel;

namespace WPFLib.Dialogs
{
	public class SimpleSelectController<T> : DispatcherPropertyChangedHelper, ISimpleController, IAsyncTreeController, IAsyncTreeItemHandler<SimpleSelectItem<T>>, IAsyncSearchListBoxController
	{
		public SimpleSelectController(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				var visualItem = new SimpleSelectItem<T>(item);
				visualItem.PropertyChangedWeak += OnItemPropertyChanged;
				Root.Add(visualItem);
			}
		}

		#region RootProperty
		public static readonly PropertyChangedEventArgs RootArgs = PropertyChangedHelper.CreateArgs<SimpleSelectController<T>>(c => c.Root);
		private readonly ObservableCollection<SimpleSelectItem<T>> _Root = new ObservableCollection<SimpleSelectItem<T>>();

		public ObservableCollection<SimpleSelectItem<T>> Root
		{
			get
			{
				return _Root;
			}
		}
		#endregion

		#region SelectedItemProperty
		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var item = sender as SimpleSelectItem<T>;
			if (item != null && item.GetIsSelectedArgs() == e && item.IsSelected)
			{
				this.SelectedItem = item;
			}
		}

		public static readonly PropertyChangedEventArgs SelectedItemArgs = PropertyChangedHelper.CreateArgs<SimpleSelectController<T>>(c => c.SelectedItem);
		private SimpleSelectItem<T> _SelectedItem;

		public SimpleSelectItem<T> SelectedItem
		{
			get
			{
				return _SelectedItem;
			}
			private set
			{
				var oldValue = SelectedItem;
				_SelectedItem = value;
				if (oldValue != value)
				{
					OnSelectedItemChanged(oldValue, value);
					OnPropertyChanged(SelectedItemArgs);
				}
			}
		}

		protected virtual void OnSelectedItemChanged(SimpleSelectItem<T> oldValue, SimpleSelectItem<T> newValue)
		{
		}
		#endregion

		#region ISimpleController
		object ISimpleController.SelectedItem
		{
			get
			{
				return this.SelectedItem;
			}
			set
			{
				this.SelectedItem = (SimpleSelectItem<T>)value;
			}
		}

		PropertyChangedEventArgs ISimpleController.SelectedItemArgs
		{
			get
			{
				return SelectedItemArgs;
			}
		}

		public void ExpandTo(object value)
		{
			foreach(var item in Root)
			{
				if (item == value)
				{
					item.IsSelected = true;
					break;
				}
			}
		}
		#endregion

		#region Tree controllers
		public Task<bool> CanChangeSelection()
		{
			return true.AsTask();
		}

		public Task LoadTreeItem(SimpleSelectItem<T> entity)
		{
			return null;
		}

		public IAsyncTreeItem GetParent(SimpleSelectItem<T> entity)
		{
			if (entity.Parent == null)
			{
				return AsyncTreeView.CreateRootItem((child) =>
				{
					var view = CollectionViewSource.GetDefaultView(this.Root);
					return view.Cast<object>().ToList().IndexOf(child);
				});
			} 
			return entity.Parent;
		}

		public int IndexOf(SimpleSelectItem<T> entity, IAsyncTreeItem child)
		{
			return CollectionViewSource.GetDefaultView(entity.ViewChildren)
				.Cast<SimpleSelectItem<T>>()
				.ToList().IndexOf((SimpleSelectItem<T>)child);
		}

		public string SortField
		{
			get
			{
				return "DisplayName";
			}
		}

		public string GetSortFieldValue(object item)
		{
			var result = item.GetType().GetProperty(SortField).GetValue(item, null);
			return (result ?? String.Empty).ToString();
		}

		public string DimensionName
		{
			get
			{
				return typeof(T).FullName;
			}
		}
		#endregion

		#region StateProperty
		public enum StateEnum
		{
			Work,
			Ready
		}

		public static readonly PropertyChangedEventArgs StateArgs = PropertyChangedHelper.CreateArgs<SimpleSelectController<T>>(c => c.State);
		private StateEnum _State = StateEnum.Ready;

		public StateEnum State
		{
			get
			{
				return _State;
			}
			protected set
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

		public Task<IEnumerable<object>> GetResults(CancellationTokenSource token, string searchQuery, int maxResults)
		{
			return Task<IEnumerable<object>>.Factory.StartNew((state) =>
				{
						var result = Root.Where(item => item.DisplayName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
						return result.AsEnumerable().Take(maxResults).OrderBy(GetSortFieldValue).ToList<object>();
				},
				token
			);
		}
	}

	public class SimpleSelectItem<T> : DispatcherPropertyChangedHelper, ITreeItemNameProvider, IAsyncTreeItem
	{
		public T Value
		{
			get;
			set;
		}

		public SimpleSelectItem(T item)
		{
			Value = item;
			_id = new Guid().ToString();
		}

		#region IsExpandedProperty
		public static readonly PropertyChangedEventArgs IsExpandedArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.IsExpanded);
		private bool _IsExpanded = true;

		public bool IsExpanded
		{
			get
			{
				return _IsExpanded;
			}
			set
			{
				var oldValue = IsExpanded;
				_IsExpanded = value;
				if (oldValue != value)
				{
					OnIsExpandedChanged(oldValue, value);
					OnPropertyChanged(IsExpandedArgs);
				}
			}
		}

		protected virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
		{
		}
		#endregion

		#region CanExpandProperty
		public static readonly PropertyChangedEventArgs CanExpandArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.CanExpand);
		private bool _CanExpand = false;

		public bool CanExpand
		{
			get
			{
				return _CanExpand;
			}
			set
			{
				var oldValue = CanExpand;
				_CanExpand = value;
				if (oldValue != value)
				{
					OnCanExpandChanged(oldValue, value);
					OnPropertyChanged(CanExpandArgs);
				}
			}
		}

		protected virtual void OnCanExpandChanged(bool oldValue, bool newValue)
		{
		}
		#endregion

		#region IsSelectedProperty
		public static readonly PropertyChangedEventArgs IsSelectedArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.IsSelected);
		private bool _IsSelected;

		public PropertyChangedEventArgs GetIsSelectedArgs()
		{
			return SimpleSelectItem<T>.IsSelectedArgs;
		}

		public bool IsSelected
		{
			get
			{
				return _IsSelected;
			}
			set
			{
				var oldValue = IsSelected;
				_IsSelected = value;
				if (oldValue != value)
				{
					OnIsSelectedChanged(oldValue, value);
					OnPropertyChanged(IsSelectedArgs);
				}
			}
		}

		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
		}
		#endregion

		#region IsLoadedProperty
		public static readonly PropertyChangedEventArgs IsLoadedArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.IsLoaded);
		private bool _IsLoaded;

		public bool IsLoaded
		{
			get
			{
				return _IsLoaded;
			}
			set
			{
				var oldValue = _IsLoaded;
				_IsLoaded = value;
				if (oldValue != value)
				{
					OnIsLoadedChanged(oldValue, value);
					OnPropertyChanged(IsLoadedArgs);
				}
			}
		}

		protected virtual void OnIsLoadedChanged(bool oldValue, bool newValue)
		{
		}
		#endregion

		#region IsSelectableProperty
		public static readonly PropertyChangedEventArgs IsSelectableArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.IsSelectable);
		private bool _IsSelectable = true;

		public bool IsSelectable
		{
			get
			{
				return _IsSelectable;
			}
			set
			{
				var oldValue = IsSelectable;
				_IsSelectable = value;
				if (oldValue != value)
				{
					OnIsSelectableChanged(oldValue, value);
					OnPropertyChanged(IsSelectableArgs);
				}
			}
		}

		protected virtual void OnIsSelectableChanged(bool oldValue, bool newValue)
		{
		}
		#endregion


		#region Hierarchy
		public static readonly PropertyChangedEventArgs ParentArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.Parent);

		public SimpleSelectItem<T> Parent
		{
			get
			{
				return null;
			}
		}

		public static readonly PropertyChangedEventArgs ViewChildrenArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.ViewChildren);
		private readonly ObservableCollection<SimpleSelectItem<T>> _ViewChildren = new ObservableCollection<SimpleSelectItem<T>>();

		public ICollection<SimpleSelectItem<T>> ViewChildren
		{
			get
			{
				return _ViewChildren;
			}
		}
		#endregion

		#region Id
		public static readonly PropertyChangedEventArgs IdArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.Id);
		private readonly string _id;

		public string Id
		{
			get
			{
				return _id;
			}
		}
		#endregion

		#region DisplayName
		public static readonly PropertyChangedEventArgs DisplayNameArgs = PropertyChangedHelper.CreateArgs<SimpleSelectItem<T>>(c => c.DisplayName);

		public string DisplayName
		{
			get
			{
				return Value != null ? Value.ToString() : Id;
			}
		}
		#endregion
	}
}

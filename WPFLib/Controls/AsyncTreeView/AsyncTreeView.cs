using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using WPFLib.Contracts;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using WPFLib.Misc;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFLib.Controls;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace WPFLib
{
	class RootTreeItem : IAsyncTreeItem
	{
		Func<IAsyncTreeItem, int> _getIndex;

		internal RootTreeItem(Func<IAsyncTreeItem, int> getIndex)
		{
			_getIndex = getIndex;
		}

		public int GetIndex(IAsyncTreeItem child)
		{
			return _getIndex(child);
		}

		public bool IsLoaded
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}

	[TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
	[TemplatePart(Name = "PART_SearchListBox", Type = typeof(AsyncSearchListBox))]
	[TemplatePart(Name = "PART_SearchIcon", Type = typeof(Image))]
	[TemplatePart(Name = "PART_ClearSearchButton", Type = typeof(Button))]
	public class AsyncTreeView : TreeView
	{
		//internal void SetSelectedContainer(TreeViewItem item)
		//{
		//    var field = typeof(TreeView).GetField("_selectedContainer", BindingFlags.Instance | BindingFlags.NonPublic);
		//    field.SetValue(this, item);
		//}

		ObservableCollection<AsyncTreeViewItem> SelectedItems = new ObservableCollection<AsyncTreeViewItem>();

		internal void OnTreeItemIsSelectedChanged(AsyncTreeViewItem item)
		{
			// Собираем выбранные элементы дерева
			// вообще в дереве должен быть выбран только один
			if (item.IsSelected && !SelectedItems.Contains(item))
			{
				SelectedItems.Add(item);
				//SetSelectedContainer(item);
			}
			else
			{
				SelectedItems.Remove(item);
			}
		}

		/// <summary>
		/// Идет процесс установки выбранного элемента
		/// </summary>
		internal bool IsRealSelect
		{
			get;
			private set;
		}

		/// <summary>
		/// Текущий элемент запрос на установку, или уже установка которого в процессе
		/// </summary>
		internal object SelectingItem = null;

		/// <summary>
		/// Запрос на выбор элемента
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal IEnumerable<IAsync> RequestSelect(object item)
		{
			// Проверка повтороного вызова
			if (SelectingItem == item)
				yield break;
			try
			{
				IsRealSelect = true;
				SelectingItem = item;

				if (this.Controller != null)
				{
					var canChange = this.Controller.CanChangeSelection();
					yield return canChange;

					// Проверка отмены текущего запроса другим запросом
					if (SelectingItem != item)
					{
						yield break;
					}

					if (!canChange.Result)
					{
						yield break;
					}
				}


				// TreeView очень умный и приходиться за него делать очистку выбранных элементов
				foreach (var selectedItem in SelectedItems.ToList())
				{
					selectedItem.IsSelected = false;
				}

				if (SelectingItem != item)
				{
					yield break;
				}

				var asyncItem = item as IAsyncTreeItem;
				if (item != null)
				{
					// Разворачиваем
					yield return ExpandToItem(asyncItem).Run();
				}
				if (SelectingItem != item)
				{
					yield break;
				}
				// Выбираем
				this.SelectedItem = item;

				// TreeView опять же очень умен, ему надо помочь
				SetBaseSelectedItem(item);
				// Ждем завершения всех Coerce методов
				yield return Throttle.Yield;
			}
			finally
			{
				// Мы закончили
				SelectingItem = null;
				IsRealSelect = false;
			}
			yield break;
		}

		public static readonly DependencyProperty IsSearchEnabledProperty = DependencyProperty.Register("IsSearchEnabled", typeof(bool), typeof(AsyncTreeView), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIsSearchEnabledChanged), DefaultValue = true });
		public bool IsSearchEnabled
		{
			get { return (bool)GetValue(IsSearchEnabledProperty); }
			set { SetValue(IsSearchEnabledProperty, value); }
		}

		#region OnIsSearchEnabledChanged
		private static void OnIsSearchEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				control.OnIsSearchEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		protected virtual void OnIsSearchEnabledChanged(bool oldValue, bool newValue)
		{
			CheckSearchEnabled();
		}
		#endregion

		AsyncSearchListBox SearchListBox;
		TextBox SearchTextBox;
		Button ClearSearchButton;

		public override void OnApplyTemplate()
		{
			SearchTextBox = this.GetTemplateChild("PART_SearchTextBox") as TextBox;
			SearchListBox = this.GetTemplateChild("PART_SearchListBox") as AsyncSearchListBox;
			ClearSearchButton = this.GetTemplateChild("PART_ClearSearchButton") as Button;

			CheckSearchEnabled();

			this.DataContextChangedObservable().Subscribe(CheckSearchEnabled);
			base.OnApplyTemplate();
		}

		void CheckSearchEnabled()
		{
			if ((IsSearchEnabled && this.DataContext is IAsyncSearchListBoxController))
			{
				InitSearch();
				VisualStateManager.GoToState(this, "SearchEnabled", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "SearchDisabled", true);
			}
		}

		bool searchInitialized = false;
		void InitSearch()
		{
			if (searchInitialized)
				return;

			searchInitialized = true;

			SearchListBox.SetBinding(ItemsControl.ItemTemplateSelectorProperty,
				new Binding() { Path = new PropertyPath(ItemsControl.ItemTemplateSelectorProperty), Source = this });
			SearchListBox.SetBinding(ItemsControl.ItemTemplateProperty,
				new Binding() { Path = new PropertyPath(ItemsControl.ItemTemplateProperty), Source = this });

			Selector.SelectedItemProperty.AddValueChangedWeak(SearchListBox, OnSearchItemSelected);

			var searchQueries = from textChanged in SearchTextBox.TextChangedObservable()
								let query = (textChanged.Sender as TextBox).Text
								from x in Observable.Return(new Unit()).Delay(TimeSpan.FromMilliseconds(300)).TakeUntil(SearchTextBox.TextChangedObservable())
								select query;

			var emptyQuery = searchQueries.Where(q => String.IsNullOrEmpty(q));
			var nonEmptyQuery = searchQueries.Where(q => !String.IsNullOrEmpty(q));

			nonEmptyQuery.ObserveOnDispatcher().Subscribe(PerformSearch);

			var clearSearchButtonClicked = ClearSearchButton.ClickObservable();
			var escPressed = this.PreviewKeyDownObservable().Where(e => e.EventArgs.Key == Key.Escape);

			clearSearchButtonClicked.Cast<object>().Merge(escPressed).Merge(emptyQuery).ObserveOnDispatcher().Subscribe(ClearSearch);
		}

		private static readonly DependencyPropertyKey IsSearchActivePropertyKey = DependencyProperty.RegisterReadOnly("IsSearchActive", typeof(bool), typeof(AsyncTreeView), new FrameworkPropertyMetadata() { DefaultValue = false });
		public static readonly DependencyProperty IsSearchActiveProperty = IsSearchActivePropertyKey.DependencyProperty;
		public bool IsSearchActive
		{
			get { return (bool)GetValue(IsSearchActiveProperty); }
			private set { SetValue(IsSearchActivePropertyKey, value); }
		}

		void OnSearchItemSelected(object sender, EventArgs args)
		{
			if (IsSearchActive)
			{
                RequestSelect(SearchListBox.SelectedItem).Run();
			}
		}

		void ClearSearch()
		{
			VisualStateManager.GoToState(this, "SearchEnabled", true);
			IsSearchActive = false;
			SearchListBox.SearchQuery = null;
			SearchTextBox.Text = null;
		}

		void PerformSearch(string query)
		{
			IsSearchActive = true;
			VisualStateManager.GoToState(this, "SearchActive", true);
			SearchListBox.SearchQuery = query;
		}

		public static IAsyncTreeItem CreateRootItem(Func<IAsyncTreeItem, int> getIndex)
		{
			return new RootTreeItem(getIndex);
		}

		public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AsyncTreeView), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnSelectedItemChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceSelectedItem), BindsTwoWayByDefault = false });

        /// <summary>
        /// С осторожностью делаем TwoWay или OneWayToSource байндинги
        /// в случае запрета смены узла может произойти ситуация что в Source будет установлен
        /// новый выбранный узел, когда на самом деле выбор был запрещен и остался неизменным
        /// Связано с работой Coerce
        /// http://connect.microsoft.com/VisualStudio/feedback/details/489775/wpf-binding-expression-updates-source-before-coercevaluecallback-is-called
        /// </summary>
		public new object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		#region OnSelectedItemChanged
		private static void OnSelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				control.OnSelectedItemChanged((object)e.OldValue, (object)e.NewValue);
		}

		protected virtual void OnSelectedItemChanged(object oldValue, object newValue)
		{
			// Мы перекрыли событие SelectedItemChanged
			// TreeView.SelectedItemChanged может вызываться не в попад
			if (SelectedItemChanged != null)
			{
				SelectedItemChanged(this, new RoutedPropertyChangedEventArgs<object>(oldValue, newValue));
			}
		}
		#endregion

		/// <summary>
		/// Перекрываем TreeView.SelectedItemChanged, который в общем то использовать в нашем случае нельзя
		/// </summary>
		public new event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

		#region OnCoerceSelectedItem
		private static object OnCoerceSelectedItem(DependencyObject o, object value)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				return control.OnCoerceSelectedItem((object)value);
			else
				return value;
		}

		protected virtual object OnCoerceSelectedItem(object value)
		{
			if (IsRealSelect)
			{
				// Настоящий выбор, принимаем значение
				return value;
			}
			else
			{
				if (value != this.SelectedItem)
				{
					// Если хотят выбрать что-то другое
					// Делаем запрос
					RequestSelect(value).Run();
				}
				// Но пока что оставляем все как есть
				//return this.SelectedItem;
                return DependencyProperty.UnsetValue;
			}
		}
		#endregion

		#region SortField
		public static readonly DependencyProperty SortFieldProperty = DependencyProperty.Register("SortField", typeof(string), typeof(AsyncTreeView), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnSortFieldChanged) });

		public string SortField
		{
			get
			{
				return (string)GetValue(SortFieldProperty);
			}
			set
			{
				SetValue(SortFieldProperty, value);
			}
		}

		private static void OnSortFieldChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				control.OnSortFieldChanged((string)e.OldValue, (string)e.NewValue);
		}

		protected virtual void OnSortFieldChanged(string oldValue, string newValue)
		{
			if (!String.IsNullOrEmpty(newValue))
			{
				this.Items.SortDescriptions.Clear();
				this.Items.SortDescriptions.Add(new SortDescription(newValue, ListSortDirection.Ascending));
			}
		}
		#endregion

		IEnumerable<IAsync> ExpandToItem(IAsyncTreeItem item)
		{
			var path = new List<int>();
			// Поулчаем пусть к элементу
			yield return GetPathForItem(this.Controller, item, path).Run();
			// Ожидаем его раскрытия
			yield return ExpandTo(path).Run();
		}

		IEnumerable<IAsync> GetPathForItem(IAsyncTreeController controller, IAsyncTreeItem item, List<int> Path)
		{
			var intf = GetInterfaceForType(item.GetType());
			if (intf != null)
			{
				var parent = intf.GetParent(controller, item);
				if (parent != null)
				{
					if (parent is RootTreeItem)
					{
						var root = parent as RootTreeItem;
						Path.Insert(0, root.GetIndex(item));
					}
					else
					{
						var parentIntf = GetInterfaceForType(parent.GetType());
						if (parentIntf != null)
						{
							var task = ExpandItem(parent);
							if (task != null)
							{
								yield return task;
							}
							Path.Insert(0, parentIntf.IndexOf(controller, parent, item));
							yield return GetPathForItem(controller, parent, Path).Run();
						}
						else
						{
							throw new Exception("Not supported type " + parent.GetType());
						}
					}
				}
			}
		}

		static AsyncTreeView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncTreeView), new FrameworkPropertyMetadata(typeof(AsyncTreeView)));
			var keyField = typeof(TreeView).GetField("SelectedItemPropertyKey", BindingFlags.Static | BindingFlags.NonPublic);
			BaseSelectedItemKey = (DependencyPropertyKey)keyField.GetValue(null);
			BaseSelectedItemKey.OverrideMetadata(typeof(AsyncTreeView), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnBaseSelectedItemChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceBaseSelectedItem) });
		}

		static DependencyPropertyKey BaseSelectedItemKey;
		void SetBaseSelectedItem(object item)
		{
			this.SetValue(BaseSelectedItemKey, item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new AsyncTreeViewItem(this);
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is AsyncTreeViewItem;
		}

		public AsyncTreeView()
		{
			this.DataContextChanged += new DependencyPropertyChangedEventHandler((s, e) => OnDataContextChanged(e.OldValue as IAsyncTreeController, e.NewValue as IAsyncTreeController));
		}

		#region OnBaseSelectedItemChanged
		private static void OnBaseSelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				control.OnBaseSelectedItemChanged((object)e.OldValue, (object)e.NewValue);
		}

		protected virtual void OnBaseSelectedItemChanged(object oldValue, object newValue)
		{
		}
		#endregion

		#region OnCoerceBaseSelectedItem
		private static object OnCoerceBaseSelectedItem(DependencyObject o, object value)
		{
			AsyncTreeView control = o as AsyncTreeView;
			if (control != null)
				return control.OnCoerceBaseSelectedItem((object)value);
			else
				return value;
		}
		#endregion

		protected virtual object OnCoerceBaseSelectedItem(object value)
		{
			// TreeView.SelectedItem всегда должен быть равен нашему выбранному элементу
			return this.SelectedItem;
		}

		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
		{
			if (IsRealSelect)
			{
				// Пропускаем события только если идет настоящий выбор
				// хотя может быть в этом уже и нет необходимости
				base.OnSelectedItemChanged(e);
			}
		}

		IAsyncTreeController Controller;

		void OnDataContextChanged(IAsyncTreeController oldValue, IAsyncTreeController newValue)
		{
			Controller = newValue;
			OnControllerChanged(oldValue, newValue);
		}

		void OnControllerChanged(IAsyncTreeController oldValue, IAsyncTreeController newValue)
		{
			if (newValue != null)
			{
				initControllerTypes = Task.Factory.StartNew(() => OnControllerChangedWorker(newValue.GetType()), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			}
			else
			{
				initControllerTypes = null;
				supportedTypes = null;
				directSupportedTypes = null;
			}
		}

		Task initControllerTypes;

		void OnControllerChangedWorker(Type controllerType)
		{
			var types = GetSupportedTypes(controllerType);
			supportedTypes = types;
			directSupportedTypes = new Dictionary<Type, ItemHandlerWrapper>();
		}

		class ItemHandlerWrapper
		{
			public ItemHandlerWrapper(Type intf)
			{
				Interface = intf;
			}

			Type Interface { get; set; }

			Type argument = null;
			public Type Argument
			{
				get
				{
					if (argument == null)
					{
						argument = Interface.GetGenericArguments()[0];
					}
					return argument;
				}
			}

			MethodInfo _loadTreeItemMethod;
			MethodInfo LoadTreeItemMethod
			{
				get
				{
					if (_loadTreeItemMethod == null)
					{
						_loadTreeItemMethod = Interface.GetMethod("LoadTreeItem");
					}
					return _loadTreeItemMethod;
				}
			}

			MethodInfo _indexOfMethod;
			MethodInfo IndexOfMethod
			{
				get
				{
					if (_indexOfMethod == null)
					{
						_indexOfMethod = Interface.GetMethod("IndexOf");
					}
					return _indexOfMethod;
				}
			}

			MethodInfo _parentProperty;
			MethodInfo ParentProperty
			{
				get
				{
					if (_parentProperty == null)
					{
						_parentProperty = Interface.GetMethod("GetParent");
					}
					return _parentProperty;
				}
			}

			public Task LoadChildren(IAsyncTreeController controller, object item)
			{
				return LoadTreeItemMethod.Invoke(controller, new object[] { item }) as Task;
			}

			public int IndexOf(IAsyncTreeController controller, IAsyncTreeItem entity, IAsyncTreeItem item)
			{
				var r = (int)IndexOfMethod.Invoke(controller, new object[] { entity, item });
                return r;
			}

			public IAsyncTreeItem GetParent(IAsyncTreeController controller, IAsyncTreeItem item)
			{
				return ParentProperty.Invoke(controller, new object[] { item }) as IAsyncTreeItem;
			}
		}

		List<ItemHandlerWrapper> supportedTypes;
		Dictionary<Type, ItemHandlerWrapper> directSupportedTypes;

		ItemHandlerWrapper GetInterfaceForType(Type dataType)
		{
			if (initControllerTypes != null && !initControllerTypes.IsCompleted)
				initControllerTypes.Wait();
			ItemHandlerWrapper intf = null;
			if (directSupportedTypes != null && !directSupportedTypes.TryGetValue(dataType, out intf))
			{
				foreach (var item in supportedTypes)
				{
					if (item.Argument.IsAssignableFrom(dataType))
					{
						directSupportedTypes[dataType] = item;
						return item;
					}
				}
			}
			return intf;
		}

		private List<ItemHandlerWrapper> GetSupportedTypes(Type controllerType)
		{
			var canLoad = typeof(IAsyncTreeItemHandler<>);
			var interfaces = from intf in controllerType.GetInterfaces()
							 where intf.IsGenericType && intf.GetGenericTypeDefinition() == canLoad
							 select intf;
			return interfaces.Select(i => new ItemHandlerWrapper(i)).ToList();
		}

		internal void OnExpanded(AsyncTreeViewItem item)
		{
			if (Controller != null)
			{
				var dataItem = item.DataContext as IAsyncTreeItem;
				var task = ExpandItem(dataItem);
				if (task != null)
				{
					item.SetWorking(true);
					task.ContinueWith((t) => Dispatcher.BeginInvoke(() => item.SetWorking(false)));
				}
			}
		}

		Task ExpandItem(IAsyncTreeItem dataItem)
		{
			if (dataItem != null && this.Controller != null && !dataItem.IsLoaded)
			{
				var intf = GetInterfaceForType(dataItem.GetType());
				if (intf == null)
					throw new Exception("Controller does not support children loading for type " + dataItem.GetType());
				var task = intf.LoadChildren(this.Controller, dataItem);
				dataItem.IsLoaded = true;
				if (task != null)
				{
					return task;
				}
			}
			return null;
		}

		private IEnumerable<IAsync> GetTreeViewItem(ItemsControl container, List<int> path)
		{
			int index = path.FirstOrDefault();
			if (container != null)
			{
				// Expand the current container
				if (container is TreeViewItem)
				{
					// Не надо тк используется GetPathForItem
					//var dataItem = container.DataContext as IAsyncTreeItem;
					//var task = ExpandItem(dataItem);
					//if (task != null)
					//{
					//    yield return task;
					//}
					if (!((TreeViewItem)container).IsExpanded)
					{
						container.SetValue(TreeViewItem.IsExpandedProperty, true);
					}
				}

				// Try to generate the ItemsPresenter and the ItemsPanel.
				// by calling ApplyTemplate.  Note that in the 
				// virtualizing case even if the item is marked 
				// expanded we still need to do this step in order to 
				// regenerate the visuals because they may have been virtualized away.

				//container.ApplyTemplate();
				ItemsPresenter itemsPresenter =
					(ItemsPresenter)container.Template.FindName("ItemsHost", container);
				if (itemsPresenter != null)
				{
					itemsPresenter.ApplyTemplate();
				}
				else
				{
					// The Tree template has not named the ItemsPresenter, 
					// so walk the descendents and find the child.
					itemsPresenter = VisualHelper.FindVisualChild<ItemsPresenter>(container);
					if (itemsPresenter == null)
					{
						container.UpdateLayout();

						itemsPresenter = VisualHelper.FindVisualChild<ItemsPresenter>(container);
					}
				}

				Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);


				// Ensure that the generator for this panel has been created.
				UIElementCollection children = itemsHostPanel.Children;

				AsyncTreeVirtualizingStackPanel virtualizingPanel =
					itemsHostPanel as AsyncTreeVirtualizingStackPanel;

				TreeViewItem subContainer;
				if (virtualizingPanel != null)
				{
                    while (container.ItemContainerGenerator.Status == GeneratorStatus.NotStarted)
                    {
                        // Тупо ожидаем когда начнется генерация
                        // иногда почему-то падает на BrinIntoView из-за того что генерация не начиналась
                        yield return Throttle.Yield;
                    }
                    
                    // Bring the item into view so 
					// that the container will be generated.
					virtualizingPanel.BringIntoView(index);

					subContainer =
						(TreeViewItem)container.ItemContainerGenerator.
						ContainerFromIndex(index);
				}
				else
				{
					subContainer =
						(TreeViewItem)container.ItemContainerGenerator.
						ContainerFromIndex(index);

					// Bring the item into view to maintain the 
					// same behavior as with a virtualizing panel.
					subContainer.BringIntoView();
				}

				if (subContainer != null)
				{
					if (path.Count > 1)
					{
						var newPath = path.ToList();
						newPath.RemoveAt(0);
						// Search the next level for the object.

						TreeViewItem resultContainer = null;
						var task = GetTreeViewItem(subContainer, newPath).Run<TreeViewItem>();
						yield return task;
						resultContainer = task.Result;

						if (resultContainer != null)
						{
							yield return resultContainer.AsResult();
						}
					}
					yield return subContainer.AsResult();
				}
			}

			yield return null;
		}

		IEnumerable<IAsync> ExpandTo(List<int> indexes)
		{
			var container = GetTreeViewItem(this, indexes).Run<TreeViewItem>();
			yield return container;
			container.Result.IsSelected = true;
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Move:
					break;

				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
					if (HasItems)
					{
						//В оригинале фреймворка тоже выбирают первый элемент
						var item = ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
						if (item != null)
						{
							item.IsSelected = true;
						}
					}
					else
					{
						//SelectedItem = null; //Если items пустая то какого лешего остается выбранный элемент
						//TODO: Я подозреваю перегрузку  AsyncTreeView.SelectedItem и ее coersion (gribkov -> sovetnikov)
					}
					return;

				case NotifyCollectionChangedAction.Replace:
					{
						base.OnItemsChanged(e);
						return;
					}
				default:
					throw new NotSupportedException();
			}

			//base.OnItemsChanged(e);
		}
	}
}

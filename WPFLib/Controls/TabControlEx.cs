using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Documents;

namespace WPFLib
{
    /// <summary>
    /// Таб контрол который умеет сохранять визуальное дерево отображаемых закладок
    /// </summary>
    public class TabControlEx : TabControl
    {
        public static readonly DependencyProperty ThrowTabItemVisualsProperty = DependencyProperty.Register("ThrowTabItemVisuals", typeof(bool), typeof(TabControlEx), new UIPropertyMetadata(false));
        public bool ThrowTabItemVisuals
        {
            get { return (bool)GetValue(ThrowTabItemVisualsProperty); }
            set { SetValue(ThrowTabItemVisualsProperty, value); }
        }

        private Panel _itemsHolder = null;

        public TabControlEx()
            : base()
        {
            // this is necessary so that we get the initial databound selected item
            this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        /// <summary>
        /// if containers are done, generate the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                UpdateSelectedItem();
            }
        }

        /// <summary>
        /// get the ItemsHolder and generate any children
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (WPFHelper.IsInDesignMode || ThrowTabItemVisuals)
                return;
            base.OnApplyTemplate();
            _itemsHolder = new Grid() { Name = "PART_ItemsHolder" }; //FIXME надо перебрасывать TemplateBinding из ContentPresenter'а в наш грид
            var cp = GetTemplateChild("PART_SelectedContentHost") as ContentPresenter;
            var holder = VisualHelper.FindVisualAncestor<Decorator>(cp);
            holder.Child = _itemsHolder;

            var marginBinding = new Binding();
            marginBinding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            marginBinding.Path = new PropertyPath(FrameworkElement.MarginProperty);
            _itemsHolder.SetBinding(FrameworkElement.MarginProperty, marginBinding);

            UpdateSelectedItem();
        }

        /// <summary>
        /// when the items change we remove any generated panel children and add any new ones as necessary
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (_itemsHolder == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _itemsHolder.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                _itemsHolder.Children.Remove(cp);
                            }
                        }
                    }

                    // don't do anything with new items because we don't want to
                    // create visuals that aren't being shown

                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }
        }

        /// <summary>
        /// update the visible child in the ItemsHolder
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        /// <summary>
        /// generate a ContentPresenter for the selected item
        /// </summary>
        void UpdateSelectedItem()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            // generate a ContentPresenter if necessary
            TabItem item = (TabItem)ItemContainerGenerator.ContainerFromItem(this.SelectedItem);

            if (item != null)
            {
                CreateChildContentPresenter(this.SelectedItem);
            }

            // show the right child
            foreach (AdornerDecorator decorator in _itemsHolder.Children)
            {
                var child = (ContentPresenter)decorator.Child;
                decorator.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void OnTabItemVisibilityChanged(object sender, EventArgs e)
        {
            // Если текущий таб скрыли
            // сделаем текущим следующий, или же если его нет то предыдущий видимый таб
            var selectedTab = GetSelectedTabItem();
            if (selectedTab != null)
            {
                if (selectedTab.Visibility != System.Windows.Visibility.Visible)
                {
                    var nextItems = this.Items
                        .Cast<object>()
                        .Select(item => (TabItem)this.ItemContainerGenerator.ContainerFromItem(item))
                        .SkipWhile(tab => tab != selectedTab)
                        .Skip(1);

                    var prevItems = this.Items
                        .Cast<object>()
                        .Reverse()
                        .Select(item => (TabItem)this.ItemContainerGenerator.ContainerFromItem(item))
                        .SkipWhile(tab => tab != selectedTab)
                        .Skip(1);

                    var items = nextItems.Concat(prevItems);
                    foreach (var nextItem in items)
                    {
                        if (nextItem.Visibility == System.Windows.Visibility.Visible)
                        {
                            nextItem.IsSelected = true;
                            return;
                        }
                    }
                    this.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        AdornerDecorator CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            var decorator = FindChildContentPresenter(item);

            if (decorator != null)
            {
                return decorator;
            }

            var tabItem = (TabItem)this.ItemContainerGenerator.ContainerFromItem(item);
            TabItem.VisibilityProperty.AddValueChangedWeak(tabItem, OnTabItemVisibilityChanged);

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            var cp = new ContentPresenter();
            cp.Content = tabItem.Content;
            cp.ContentTemplate = this.SelectedContentTemplate;
            cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
            cp.ContentStringFormat = this.SelectedContentStringFormat;
            cp.Tag = tabItem;

            decorator = new AdornerDecorator() { Child = cp };
            decorator.Visibility = Visibility.Collapsed;

            decorator.Tag = item;

            _itemsHolder.Children.Add(decorator);
            return decorator;
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        AdornerDecorator FindChildContentPresenter(object data)
        {
            if (data == null)
            {
                return null;
            }

            if (_itemsHolder == null)
            {
                return null;
            }

            foreach (AdornerDecorator decorator in _itemsHolder.Children)
            {
                if (decorator.Tag == data)
                    return decorator;
            }

            return null;
        }

        /// <summary>
        /// copied from TabControl; wish it were protected in that class instead of private
        /// </summary>
        /// <returns></returns>
        protected TabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            TabItem item = selectedItem as TabItem;
            if (item == null)
            {
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;
            }
            return item;
        }
    }

}

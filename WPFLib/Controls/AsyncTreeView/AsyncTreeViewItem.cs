using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using WPFLib.Contracts;

namespace WPFLib
{
    public class AsyncTreeViewItem : TreeViewItem
    {
        #region OnIsSelectedChanged
        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AsyncTreeViewItem control = o as AsyncTreeViewItem;
            if (control != null)
                control.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            //Debug.WriteLine("OnIsSelectedChanged " + newValue + " " + this.GetHashCode());
            // Посылаем нотификации дереву
            Tree.OnTreeItemIsSelectedChanged(this);
        }
        #endregion

        #region OnCoerceIsSelected
        private static object OnCoerceIsSelected(DependencyObject o, object value)
        {
            AsyncTreeViewItem control = o as AsyncTreeViewItem;
            if (control != null)
                return control.OnCoerceIsSelected((bool)value);
            else
                return value;
        }
        #endregion

        protected virtual bool OnCoerceIsSelected(bool value)
        {
            if (Tree.IsRealSelect)
            {
                // В случае процесса выбора
                // выбран будет полюбому только тот элемент который выбирает дерево
                return this.DataContext == Tree.SelectingItem;
            }
            else
            {
                // Проверка на IAsyncTreeItem нужна тк AsyncTreeItem может быть в состоянии удаления из визуального дерева
                // в этом случае в дата контексте будет находиться MS.Internal.NamedObject, соответстенно все дальше упадет
                if (value && this.DataContext is IAsyncTreeItem)
                {
                    // Если хотят выбрать кого-то, посылаем запрос
                    Tree.RequestSelect(this.DataContext).Run();
                }
                // И пока сотавляем все как есть
                return IsSelected;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            // Дереву не надо ничего видить
            // мы все сделаем за него
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Вместо OnGotFocus будем запускать выбор элемента по клику
            Tree.RequestSelect(this.DataContext).Run();
            base.OnMouseLeftButtonDown(e);
        }

        static AsyncTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncTreeViewItem), new FrameworkPropertyMetadata(typeof(AsyncTreeViewItem)));
            IsSelectedProperty.OverrideMetadata(typeof(AsyncTreeViewItem), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIsSelectedChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceIsSelected) });
        }

        public static readonly DependencyProperty CanExpandProperty = DependencyProperty.Register("CanExpand", typeof(bool), typeof(AsyncTreeViewItem), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnCanExpandChanged), DefaultValue = true });
        public bool CanExpand
        {
            get { return (bool)GetValue(CanExpandProperty); }
            set { SetValue(CanExpandProperty, value); }
        }

        #region OnCanExpandChanged
        private static void OnCanExpandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AsyncTreeViewItem control = o as AsyncTreeViewItem;
            if (control != null)
                control.OnCanExpandChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnCanExpandChanged(bool oldValue, bool newValue)
        {
        }
        #endregion 

        private static readonly DependencyPropertyKey WorkingPropertyKey = DependencyProperty.RegisterReadOnly("Working", typeof(bool), typeof(AsyncTreeViewItem), new UIPropertyMetadata());
        public static readonly DependencyProperty WorkingProperty = WorkingPropertyKey.DependencyProperty;
        public bool Working
        {
            get { return (bool)GetValue(WorkingProperty); }
            private set { SetValue(WorkingPropertyKey, value); }
        }

        internal void SetWorking(bool working)
        {
            Working = working;
        }

        protected AsyncTreeView Tree;
        internal AsyncTreeViewItem(AsyncTreeView tree)
        {
            Tree = tree;
			if (!String.IsNullOrEmpty(tree.SortField))
			{
				this.Items.SortDescriptions.Add(new SortDescription(tree.SortField, ListSortDirection.Ascending));
			}
		}

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AsyncTreeViewItem(Tree);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AsyncTreeViewItem;
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {
            Tree.OnExpanded(this);
            base.OnExpanded(e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Data;

namespace WPFLib
{
	public class TreeListViewItem : AsyncTreeViewItem
    {

        public static readonly DependencyProperty SeparatorStyleProperty;
        public Style SeparatorStyle
        {
            get { return (Style)GetValue(SeparatorStyleProperty); }
            set { SetValue(SeparatorStyleProperty, value); }
        }

        private static readonly Style DefaultSeparatorStyle;

        static TreeListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));

            DefaultSeparatorStyle = new Style(typeof(Rectangle));
            DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
            SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(TreeListViewItem),
                                                                    new UIPropertyMetadata(DefaultSeparatorStyle));

        }

        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }

		internal TreeListViewItem(TreeListView tree) : base(tree)
		{
            var b = new Binding() { Path = new PropertyPath(TreeListView.SeparatorStyleProperty), Source = tree };
            this.SetBinding(TreeListViewItem.SeparatorStyleProperty, b);
			if (!String.IsNullOrEmpty(tree.SortField))
			{
				this.Items.SortDescriptions.Add(new SortDescription(tree.SortField, ListSortDirection.Ascending));
			}
		}

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem((TreeListView)Tree);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private int _level = -1;
    }
}

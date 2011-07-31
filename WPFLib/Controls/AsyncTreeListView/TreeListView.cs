using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Media;
using System.Windows.Data;

namespace WPFLib
{
	public class TreeListView : AsyncTreeView
    {
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));

            //ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
            //template.Seal();
            //ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(template));

            DefaultSeparatorStyle = new Style(typeof(Rectangle));
            DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
            SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(TreeListView),
                                                                    new UIPropertyMetadata(DefaultSeparatorStyle, SeparatorStyleChanged));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem(this);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }

        #region Columns

        /// <summary> GridViewColumn List</summary>
        public GridViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new GridViewColumnCollection();
                }

                return _columns;
            }
        }

        private GridViewColumnCollection _columns;

        #endregion

		#region SortField

		public static readonly DependencyProperty SortFieldProperty = DependencyProperty.Register("SortField", typeof(string), typeof(TreeListView), new UIPropertyMetadata()
		{
			PropertyChangedCallback = new PropertyChangedCallback(OnSortFieldChanged)
		});

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
			TreeListView control = o as TreeListView;
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

        ExtendedGridViewHeaderRowPresenter _headerRowPresenter;
        ExtendedGridViewHeaderRowPresenter HeaderRowPresenter
        {
            get
            {
                if (_headerRowPresenter == null)
                {
                    _headerRowPresenter = VisualHelper.FindVisualChild<ExtendedGridViewHeaderRowPresenter>(this, "PART_HeaderRowPresenter");
                    _headerRowPresenter.Arrange += new EventHandler(_headerRowPresenter_Arrange);
                }
                return _headerRowPresenter;
            }
        }

        void _headerRowPresenter_Arrange(object sender, EventArgs e)
        {
            if (VertLines != null)
            {
                VertLines.InvalidateArrange();
            }
        }

        TreeListViewVertLines _vertLines;
        TreeListViewVertLines VertLines
        {
            get
            {
                if (_vertLines == null)
                {
                    _vertLines = VisualHelper.FindVisualChild<TreeListViewVertLines>(this, "PART_VertLines");
                }
                return _vertLines;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (VertLines != null)
            {
                //VertLines.InvalidateArrange();
                if (VertLines.HeaderRowPresenter == null && HeaderRowPresenter != null)
                {
                    VertLines.HeaderRowPresenter = HeaderRowPresenter;
                }
            }
            var size = base.ArrangeOverride(arrangeBounds);

            return size;
        }

        private static readonly Style DefaultSeparatorStyle;
        public static readonly DependencyProperty SeparatorStyleProperty;
        private readonly List<FrameworkElement> _lines = new List<FrameworkElement>();

        public Style SeparatorStyle
        {
            get { return (Style)GetValue(SeparatorStyleProperty); }
            set { SetValue(SeparatorStyleProperty, value); }
        }

        private IEnumerable<FrameworkElement> Children
        {
            get { return LogicalTreeHelper.GetChildren(this).OfType<FrameworkElement>(); }
        }

        private static void SeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;

namespace WPFLib
{
    public class TreeGridViewRowPresenter : GridViewRowPresenter
    {
        private static readonly Style DefaultSeparatorStyle;
        public static readonly DependencyProperty SeparatorStyleProperty;
        private readonly List<FrameworkElement> _lines = new List<FrameworkElement>();


		public static DependencyProperty FirstColumnIndentProperty = DependencyProperty.Register("FirstColumnIndent", typeof(Double), typeof(TreeGridViewRowPresenter), new PropertyMetadata(0d));
		public static DependencyProperty ExpanderProperty = DependencyProperty.Register("Expander", typeof(UIElement), typeof(TreeGridViewRowPresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnExpanderChanged)));

        private UIElementCollection childs;

        private static PropertyInfo ActualIndexProperty = typeof(GridViewColumn).GetProperty("ActualIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        private static PropertyInfo DesiredWidthProperty = typeof(GridViewColumn).GetProperty("DesiredWidth", BindingFlags.NonPublic | BindingFlags.Instance);

        public TreeGridViewRowPresenter()
        {
            childs = new UIElementCollection(this, this);
        }

        static TreeGridViewRowPresenter()
        {
            DefaultSeparatorStyle = new Style(typeof(Rectangle));
            DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
            SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(TreeGridViewRowPresenter),
                                                                    new UIPropertyMetadata(DefaultSeparatorStyle, SeparatorStyleChanged));
        }

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
            var presenter = (TreeGridViewRowPresenter)d;
            var style = (Style)e.NewValue;
            foreach (FrameworkElement line in presenter._lines)
            {
                line.Style = style;
            }
        }

        private void EnsureLines(int count)
        {
            count = count - _lines.Count;
            for (var i = 0; i < count; i++)
            {
                var line = (FrameworkElement)Activator.CreateInstance(SeparatorStyle.TargetType);
                line = new Rectangle { Fill = Brushes.LightGray };
                line.Style = SeparatorStyle;
                AddVisualChild(line);
                _lines.Add(line);
            }
        }

        public Double FirstColumnIndent
        {
            get { return (Double)this.GetValue(FirstColumnIndentProperty); }
            set { this.SetValue(FirstColumnIndentProperty, value); }
        }

        public UIElement Expander
        {
            get { return (UIElement)this.GetValue(ExpanderProperty); }
            set { this.SetValue(ExpanderProperty, value); }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size s = base.ArrangeOverride(arrangeSize);

            if (this.Columns == null || this.Columns.Count == 0) return arrangeSize;
            UIElement expander = this.Expander;

            double current = 0;
            double max = arrangeSize.Width;
            for (int x = 0; x < this.Columns.Count; x++)
            {
                GridViewColumn column = this.Columns[x];
                // Actual index needed for column reorder
                UIElement uiColumn = (UIElement)base.GetVisualChild((int)ActualIndexProperty.GetValue(column, null));

                // Compute column width
                double w = Math.Min(max, (Double.IsNaN(column.Width)) ? (double)DesiredWidthProperty.GetValue(column, null) : column.Width);

                // First column indent
                if (x == 0 && expander != null)
                {
                    double indent = FirstColumnIndent + expander.DesiredSize.Width;
                    var cp = uiColumn as ContentPresenter;
                    if (cp != null)
                    {
                        cp.Margin = new Thickness(6 + indent, cp.Margin.Top, cp.Margin.Right, cp.Margin.Bottom);
                    }
                    //uiColumn.Arrange(new Rect(current /*+ indent*/, 0, w/*Math.Max(0, w - indent)*/, arrangeSize.Height));
                }
                //else
                    uiColumn.Arrange(new Rect(current, 0, w, arrangeSize.Height));
                max -= w;
                current += w;
            }

            // Show expander
            if (expander != null)
            {
                expander.Arrange(new Rect(this.FirstColumnIndent, 0, expander.DesiredSize.Width, expander.DesiredSize.Height));
            }

            //var size = arrangeSize;
            //var children = Children.ToList();
            //EnsureLines(children.Count);
            //for (var i = 0; i < _lines.Count; i++)
            //{
            //    var child = children[i];
            //    var x = child.TransformToAncestor(this).Transform(new Point(child.ActualWidth, 0)).X + child.Margin.Right;
            //    var rect = new Rect(x, -Margin.Top - 2, 1, size.Height + Margin.Top + Margin.Bottom + 4);
            //    var line = _lines[i];
            //    line.Measure(rect.Size);
            //    line.Arrange(rect);
            //}

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size s = base.MeasureOverride(constraint);
            //Size desired = new Size();

            //for (int i = 0; i < VisualChildrenCount; i++)
            //{
            //    var e = this.GetVisualChild(i) as UIElement;
            //    if (e != null)
            //    {
            //        e.Measure(s);
            //        desired.Width += e.DesiredSize.Width;
            //        desired.Height = Math.Max(e.DesiredSize.Height, desired.Height);
            //    }
            //}

            // Measure expander
            UIElement expander = this.Expander;
            if (expander != null)
            {
                // Compute max measure
                expander.Measure(constraint);
                s.Width += expander.DesiredSize.Width; //Math.Max(s.Width, expander.DesiredSize.Width);
                s.Height = Math.Max(s.Height, expander.DesiredSize.Height);
                //desired.Width += expander.DesiredSize.Width; //Math.Max(s.Width, expander.DesiredSize.Width);
                //desired.Height = Math.Max(desired.Height, expander.DesiredSize.Height);
            }

            return s;
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            // Last element is always the expander
            // called by render engine
            if (index < base.VisualChildrenCount)
                return base.GetVisualChild(index);
            else if (index == base.VisualChildrenCount)
            {
                return this.Expander;
            }
            else
            {
                return _lines[index - (base.VisualChildrenCount + 1)];
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                // Last element is always the expander
                if (this.Expander != null)
                    return base.VisualChildrenCount + 1 + _lines.Count;
                else
                    return base.VisualChildrenCount + _lines.Count;
            }
        }

        private static void OnExpanderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Use a second UIElementCollection so base methods work as original
			TreeGridViewRowPresenter p = (TreeGridViewRowPresenter)d;

            p.childs.Remove(e.OldValue as UIElement);
            p.childs.Add((UIElement)e.NewValue);
        }

    }
}

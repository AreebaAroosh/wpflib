using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Diagnostics;

namespace WPFLib
{
    public class TreeListViewVertLines : FrameworkElement
    {
        public FrameworkElement HeaderRowPresenter { get; set; }

        static TreeListViewVertLines()
        {
            SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(TreeListViewVertLines),
                                                                    new UIPropertyMetadata(null, SeparatorStyleChanged));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            var children = VisualHelper.FindVisualChildrenOneLevel<FrameworkElement>(HeaderRowPresenter).Where(el => el.Visibility == System.Windows.Visibility.Visible).ToList();
            EnsureLines(children.Count);
			var linesCount = _lines.Count;
            for (var i = 1; i < linesCount; i++)
            {
				if (i < children.Count)
				{
					var child = children[i];
					var x = child.TransformToVisual(this).Transform(new Point(child.ActualWidth, 0)).X + child.Margin.Right;
					var y = child.TransformToVisual(this).Transform(new Point(0, child.ActualHeight)).Y;
					var rect = new Rect(x - 0.5 /*опытным путем*/, y - Margin.Top, 1, size.Height + Margin.Top + Margin.Bottom);
					var line = _lines[i];
					line.Measure(rect.Size);
					line.Arrange(rect);
				}
				else
				{
                    var line = _lines[_lines.Count - 1];
					_lines.RemoveAt(_lines.Count - 1);
                    RemoveVisualChild(line);
				}
            }

            return size;
        }

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
            var presenter = (TreeListViewVertLines)d;
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
                line.Style = SeparatorStyle;
                AddVisualChild(line);
                _lines.Add(line);
            }
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            // Last element is always the expander
            // called by render engine
            if (index < base.VisualChildrenCount)
            {
                return base.GetVisualChild(index);
            }
            else
            {
                return _lines[index - (base.VisualChildrenCount)];
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + _lines.Count;
            }
        }
    }
}

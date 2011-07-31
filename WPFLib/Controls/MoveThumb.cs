using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Pochta.UI
{
	public class ResizeThumbSimple:Thumb
	{
		public Grid TrackedControl
		{
			get { return (Grid)GetValue(TrackedControlProperty); }
			set { SetValue(TrackedControlProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TrackedControl.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TrackedControlProperty =
			DependencyProperty.Register("TrackedControl", typeof(Grid), typeof(ResizeThumbSimple));



		public ResizeThumbSimple()
		{
			DragDelta += ResizeDelta;
		}

		private void ResizeDelta(object sender, DragDeltaEventArgs e)
		{
			double horizontalChange = e.HorizontalChange;
			double verticalChange = e.VerticalChange;
			switch (HorizontalAlignment)
			{
				case HorizontalAlignment.Left:
					double h = Math.Min(horizontalChange, TrackedControl.ActualWidth - TrackedControl.MinWidth);
					//Canvas.SetLeft(TrackedControl, Canvas.GetLeft(TrackedControl) + h);
					TrackedControl.Width -= h;
					return;
				case HorizontalAlignment.Right:
					h = Math.Min(-horizontalChange, TrackedControl.ActualWidth - TrackedControl.MinWidth);
					TrackedControl.Width -= h;
					return;
				default:
					break;
			}
			switch (VerticalAlignment)
			{
				case VerticalAlignment.Top:
					double v = Math.Min(verticalChange, TrackedControl.ActualHeight - TrackedControl.MinHeight);
					Canvas.SetTop(TrackedControl, v);
					TrackedControl.Height -= v;
					return;
				case VerticalAlignment.Bottom:
					v = Math.Min(verticalChange, TrackedControl.ActualHeight - TrackedControl.MinHeight);
					TrackedControl.Height -= v;
					break;
				default:
					break;
			}
		}
	}
}

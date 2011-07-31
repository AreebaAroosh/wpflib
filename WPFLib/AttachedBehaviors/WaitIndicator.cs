using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WPFLib.Misc;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace WPFLib
{
    public class WaitIndicatorAdorner : Adorner
    {
        public WaitIndicatorAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            waitIndicator = new WaitIndicatorControl() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            AddVisualChild(waitIndicator);
        }

        WaitIndicatorControl waitIndicator;

        protected override int VisualChildrenCount
        {
            get { return waitIndicator != null ? 1 : 0; }
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (index == 0 && waitIndicator != null)
                return waitIndicator;

            return base.GetVisualChild(index);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (waitIndicator != null)
            {
                waitIndicator.Arrange(new Rect(new Point(0, 0), AdornedElement.RenderSize));
            }

            return finalSize;
        }
    }


    public static class WaitIndicator
    {
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.RegisterAttached("Visible", typeof(bool), typeof(WaitIndicator), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnVisibleChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceVisible) });

        public static void SetVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(VisibleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(VisibleProperty);
        }

        #region OnVisibleChanged
        private static void OnVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnVisibleChanged((bool)e.NewValue, o as FrameworkElement);
        }
        #endregion

        private static void OnVisibleChanged(bool visible, FrameworkElement el)
        {
            if (!el.IsLoaded)
            {
                el.Loaded += new RoutedEventHandler(OnElementLoaded);
            }
            else
            {
                var layer = AdornerLayer.GetAdornerLayer(el);
                if (visible)
                {
					if (el.Visibility == Visibility.Visible)
					{
						layer.Add(new WaitIndicatorAdorner(el));
						HideByAnimation(el);
					}
					else
					{
					}
                }
                else
                {
                	if (layer != null)
                	{
                        var adorners = layer.GetAdorners(el);
                        if (adorners != null)
                        {
                            var adorner = adorners.Where(a => a is WaitIndicatorAdorner).FirstOrDefault();
                            if (adorner != null)
                            {
                                layer.Remove(adorner);
                                RestoreByAnimation(el);
                            }
                        }
                	}
                }
            }

        }

        static void HideByAnimation(IAnimatable el)
        {
            var an = new ObjectAnimationUsingKeyFrames();
            an.BeginTime = TimeSpan.Zero;
            an.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            el.BeginAnimation(UIElement.VisibilityProperty, an);
        }

        static void RestoreByAnimation(IAnimatable el)
        {
            el.BeginAnimation(UIElement.VisibilityProperty, null);
        }

        static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var el = sender as FrameworkElement;
            el.Loaded -= new RoutedEventHandler(OnElementLoaded);
            OnVisibleChanged(GetVisible(el), el);
        }

        #region OnCoerceVisible
        private static object OnCoerceVisible(DependencyObject o, object value)
        {
            return value;
        }
        #endregion
    }
}

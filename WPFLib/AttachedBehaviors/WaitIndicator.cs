using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

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

        public void SetText(string text)
        {
            waitIndicator.Text = text;
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



        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(WaitIndicator), new UIPropertyMetadata(OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnTextChanged((string)e.NewValue, d as FrameworkElement);
        }

        private static void OnTextChanged(string newValue, FrameworkElement frameworkElement)
        {
            if (frameworkElement != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(frameworkElement);
                if (layer != null)
                {
                    Adorner[] adorners = layer.GetAdorners(frameworkElement);
                    if (adorners != null)
                    {
                        var adorner =
                            (WaitIndicatorAdorner)
                            adorners.Where(a => a as WaitIndicatorAdorner != null).FirstOrDefault();
                        if (adorner != null)
                        {
                            adorner.SetText(newValue);
                        }
                    }
                }
            }
        }


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
                    var waitIndicatorAdorner = GetAdornerFromElement(el);
                    if (waitIndicatorAdorner == null)
                    {
                        waitIndicatorAdorner = new WaitIndicatorAdorner(el);
                        layer.Add(waitIndicatorAdorner);
                    }
                    waitIndicatorAdorner.SetText(GetText(el));
                    el.Visibility = Visibility.Hidden;
                }
                else
                {
                    var adorner = GetAdornerFromElement(el);
                    if (adorner != null)
                    {
                        layer.Remove(adorner);
                        el.Visibility = Visibility.Visible;
                    }
                }
            }

        }

        private static WaitIndicatorAdorner GetAdornerFromElement(FrameworkElement el)
        {
            var layer = AdornerLayer.GetAdornerLayer(el);
            var adorners = layer.GetAdorners(el);
            if (adorners != null)
            {
                return adorners.Where(a => a is WaitIndicatorAdorner).FirstOrDefault() as WaitIndicatorAdorner;
            }
            return null;
        }

        static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var el = sender as FrameworkElement;
            el.Loaded -= new RoutedEventHandler(OnElementLoaded);
            OnVisibleChanged(GetVisible(el), el);
            OnTextChanged(GetText(el), el);
        }

        #region OnCoerceVisible
        private static object OnCoerceVisible(DependencyObject o, object value)
        {
            return value;
        }
        #endregion
    }
}

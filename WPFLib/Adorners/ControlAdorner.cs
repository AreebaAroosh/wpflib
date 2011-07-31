using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFLib
{
    public class ControlAdorner : Adorner
    {
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.RegisterAttached("Visible", typeof(bool), typeof(ControlAdorner), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnVisibleChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceVisible) });

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
            OnVisibleChanged((bool)e.NewValue, (FrameworkElement)o);
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
                ControlAdorner.Show(el, visible);
            }

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

        public static readonly DependencyProperty CurrentControlAdornerProperty = DependencyProperty.RegisterAttached("CurrentControlAdorner", typeof(ControlAdorner), typeof(ControlAdorner), new FrameworkPropertyMetadata());

        public static void SetCurrentControlAdorner(DependencyObject obj, ControlAdorner value)
        {
            obj.SetValue(CurrentControlAdornerProperty, value);
        }

        public static ControlAdorner GetCurrentControlAdorner(DependencyObject obj)
        {
            return (ControlAdorner)obj.GetValue(CurrentControlAdornerProperty);
        }
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.RegisterAttached("Template", typeof(ControlTemplate), typeof(ControlAdorner), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnTemplateChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceTemplate) });

        public static void SetTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ControlTemplate GetTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(TemplateProperty);
        }

        #region OnTemplateChanged
        private static void OnTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region OnCoerceTemplate
        private static object OnCoerceTemplate(DependencyObject o, object value)
        {
            return value;
        }
        #endregion

        public static void Show(DependencyObject adornerSite, bool show)
        {
            UIElement visual = adornerSite as UIElement;
            if (visual != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(visual);
                if (adornerLayer != null)
                {
                    ControlAdorner adorner = GetCurrentControlAdorner(visual) as ControlAdorner;
                    if (show && (adorner == null))
                    {
                        ControlTemplate errorTemplate = GetTemplate(visual);
                        if (errorTemplate != null)
                        {
                            adorner = new ControlAdorner(visual, errorTemplate);
                            adornerLayer.Add(adorner);
                            SetCurrentControlAdorner(visual, adorner);
                        }
                    }
                    else if (!show && (adorner != null))
                    {
                        adorner.ClearChild();
                        adornerLayer.Remove(adorner);
                        visual.ClearValue(CurrentControlAdornerProperty);
                    }
                }
            }
        }

        private Control _child;
        private FrameworkElement _referenceElement;

        public ControlAdorner(UIElement adornedElement, ControlTemplate adornerTemplate)
            : base(adornedElement)
        {
            Control control = new Control();
            //control.DataContext = Validation.GetErrors(adornedElement);
            control.Template = adornerTemplate;
            this._child = control;
            base.AddVisualChild(this._child);
        }

        protected override Size ArrangeOverride(Size size)
        {
            Size size2 = base.ArrangeOverride(size);
            if (this._child != null)
            {
                this._child.Arrange(new Rect(new Point(), size2));
            }
            return size2;
        }

        public void ClearChild()
        {
            base.RemoveVisualChild(this._child);
            this._child = null;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            if (this.ReferenceElement == null)
            {
                return transform;
            }
            GeneralTransformGroup group = new GeneralTransformGroup();
            group.Children.Add(transform);
            GeneralTransform transform2 = base.TransformToDescendant(this.ReferenceElement);
            if (transform2 != null)
            {
                group.Children.Add(transform2);
            }
            return group;
        }

        protected override Visual GetVisualChild(int index)
        {
            if ((this._child == null) || (index != 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return this._child;
        }

        bool AreClose(Size x, Size y)
        {
            return Math.Abs(x.Width - y.Width) <= Double.Epsilon;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (((this.ReferenceElement != null) && (base.AdornedElement != null)) && (base.AdornedElement.IsMeasureValid &&
                !AreClose(this.ReferenceElement.DesiredSize, base.AdornedElement.DesiredSize)))
            {
                this.ReferenceElement.InvalidateMeasure();
            }
            this._child.Measure(constraint);
            return this._child.DesiredSize;
        }

        public FrameworkElement ReferenceElement
        {
            get
            {
                return this._referenceElement;
            }
            set
            {
                this._referenceElement = value;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (this._child == null)
                {
                    return 0;
                }
                return 1;
            }
        }
    }

}

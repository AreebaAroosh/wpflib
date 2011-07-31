using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections;

namespace WPFLib
{
    [ContentProperty("Child")]
    public class AdornedControlPlaceholder : FrameworkElement, IAddChild
    {
        private UIElement _child;
        private ControlAdorner _templatedAdorner;

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            UIElement child = this.Child;
            if (child != null)
            {
                child.Arrange(new Rect(arrangeBounds));
            }
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            if ((this._child == null) || (index != 0))
            {
                throw new ArgumentOutOfRangeException("Visual_ArgumentOutOfRange");
            }
            return this._child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (base.TemplatedParent == null)
            {
                throw new InvalidOperationException("AdornedElementPlaceholderMustBeInTemplate");
            }
            if (this.AdornedElement == null)
            {
                return new Size(0.0, 0.0);
            }
            Size renderSize = this.AdornedElement.RenderSize;
            UIElement child = this.Child;
            if (child != null)
            {
                child.Measure(renderSize);
            }
            return renderSize;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (base.TemplatedParent == null)
            {
                throw new InvalidOperationException("AdornedElementPlaceholderMustBeInTemplate");
            }
            base.OnInitialized(e);
        }

        void IAddChild.AddChild(object value)
        {
            if (value != null)
            {
                if (!(value is UIElement))
                {
                    throw new ArgumentException("UnexpectedParameterType");
                }
                if (this.Child != null)
                {
                    throw new ArgumentException("CanOnlyHaveOneChild");
                }
                this.Child = (UIElement)value;
            }
        }

        void IAddChild.AddText(string text)
        {
            //XamlSerializerUtil.ThrowIfNonWhiteSpaceInAddText(text, this);
        }

        // Properties
        public UIElement AdornedElement
        {
            get
            {
                if (this.TemplatedAdorner != null)
                {
                    return this.TemplatedAdorner.AdornedElement;
                }
                return null;
            }
        }

        [DefaultValue((string)null)]
        public virtual UIElement Child
        {
            get
            {
                return this._child;
            }
            set
            {
                UIElement child = this._child;
                if (child != value)
                {
                    base.RemoveVisualChild(child);
                    base.RemoveLogicalChild(child);
                    this._child = value;
                    base.AddVisualChild(this._child);
                    base.AddLogicalChild(value);
                    base.InvalidateMeasure();
                }
            }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                var l = new List<object>();
                l.Add(this._child);
                var en = l as IEnumerable;
                return en.GetEnumerator();
            }
        }

        private ControlAdorner TemplatedAdorner
        {
            get
            {
                if (this._templatedAdorner == null)
                {
                    FrameworkElement templatedParent = base.TemplatedParent as FrameworkElement;
                    if (templatedParent != null)
                    {
                        this._templatedAdorner = VisualTreeHelper.GetParent(templatedParent) as ControlAdorner;
                        if ((this._templatedAdorner != null) && (this._templatedAdorner.ReferenceElement == null))
                        {
                            this._templatedAdorner.ReferenceElement = this;
                        }
                    }
                }
                return this._templatedAdorner;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (this._child != null)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

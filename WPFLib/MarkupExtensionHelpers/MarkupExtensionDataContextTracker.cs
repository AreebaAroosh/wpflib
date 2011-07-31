using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFLib.MarkupExtensionHelpers
{
    public abstract class MarkupExtensionDataContextTracker
    {
        protected FrameworkElement TargetElement
        {
            get;
            private set;
        }

        protected DependencyProperty TargetProperty
        {
            get;
            private set;
        }

        public MarkupExtensionDataContextTracker(FrameworkElement _targetElement, DependencyProperty _targetProperty)
        {
            TargetElement = _targetElement;
            TargetProperty = _targetProperty;

            TargetElement.DataContextChanged += new DependencyPropertyChangedEventHandler(OnTargetDataContextChanged);
        }

        protected void InitialDataContextCheck()
        {
            if (TargetElement.DataContext != null)
            {
                OnTargetDataContextChanged();
            }
        }

        void OnTargetDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnTargetDataContextChanged();
        }

        protected abstract void OnTargetDataContextChanged();
    }
}

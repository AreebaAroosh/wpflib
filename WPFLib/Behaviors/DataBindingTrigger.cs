using System.Windows;
using System.Windows.Data;
using Microsoft.Expression.Interactivity;
using System.Windows.Interactivity;
using System;
using System.ComponentModel;
using System.Threading;

namespace WPFLib
{
    internal class BindingValueCache
    {
        internal readonly Type BindingValueType;
        internal readonly object ValueAsBindingValueType;

        internal BindingValueCache(Type bindingValueType, object valueAsBindingValueType)
        {
            this.BindingValueType = bindingValueType;
            this.ValueAsBindingValueType = valueAsBindingValueType;
        }
    }

    public class DataBindingTrigger : TriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataBindingTrigger), new PropertyMetadata(null, DataBindingTrigger.HandleValueChanged));
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataBindingTrigger), new PropertyMetadata(null, DataBindingTrigger.HandleBindingValueChanged));
        public static readonly DependencyProperty TriggerOnRisingProperty = DependencyProperty.Register("TriggerOnRising", typeof(bool), typeof(DataBindingTrigger), new PropertyMetadata(true));


        public DataBindingTrigger()
        {
            BindingValueCache = new BindingValueCache(null, null);
        }

        public bool TriggerOnRising
        {
            get { return (bool)this.GetValue(DataBindingTrigger.TriggerOnRisingProperty); }
            set { this.SetValue(DataBindingTrigger.TriggerOnRisingProperty, value); }
        }

        public object Value
        {
            get { return (object)this.GetValue(DataBindingTrigger.ValueProperty); }
            set { this.SetValue(DataBindingTrigger.ValueProperty, value); }
        }

        public object Binding
        {
            get { return (object)this.GetValue(DataBindingTrigger.BindingProperty); }
            set { this.SetValue(DataBindingTrigger.BindingProperty, value); }
        }

        bool isAttached = false;
        bool needCheck = false;

        protected override void OnAttached()
        {
            isAttached = true;
            //if (needCheck)
            //{
                //this.CheckState();
                this.Dispatcher.BeginInvoke((Action)this.CheckState, System.Windows.Threading.DispatcherPriority.Render);
            //}
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private static void HandleBindingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((DataBindingTrigger)sender).OnBindingValueChanged(e);
        }

        protected virtual void OnBindingValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CheckState();
        }

        private static void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((DataBindingTrigger)sender).OnValueChanged(e);
        }

        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CheckState();
        }

        BindingValueCache BindingValueCache;

        private void CheckState()
        {
            object referenceValue = this.Value;
            string text = referenceValue as string;
            Type type = (Binding != null) ? Binding.GetType() : null;
            if (((text != null) && (type != null)) && (type != typeof(string)))
            {
                BindingValueCache bindingValueCache = this.BindingValueCache;
                Type bindingValueType = bindingValueCache.BindingValueType;
                object valueAsBindingValueType = bindingValueCache.ValueAsBindingValueType;
                if (type != bindingValueType)
                {
                    valueAsBindingValueType = referenceValue;
                    TypeConverter converter = TypeDescriptor.GetConverter(type);

                    if ((converter != null) && converter.CanConvertFrom(typeof(string)))
                    {
                        valueAsBindingValueType = converter.ConvertFromString(null, Thread.CurrentThread.CurrentUICulture, text);
                    }
                    bindingValueCache = new BindingValueCache(type, valueAsBindingValueType);
                    this.BindingValueCache = bindingValueCache;
                }
                referenceValue = valueAsBindingValueType;
            }
            this.IsTrue = object.Equals(Binding, referenceValue);
        }

        private bool isTrue;

        private bool IsTrue
        {
            get { return this.isTrue; }
            set
            {
                if (this.isTrue != value)
                {
                    this.isTrue = value;

                    if (this.IsTrue == this.TriggerOnRising)
                    {
                        if (!isAttached)
                        {
                            needCheck = true;
                        }
                        else
                        {
                            this.InvokeActions(this.Binding);
                        }
                    }
                }
            }
        }
    }
}

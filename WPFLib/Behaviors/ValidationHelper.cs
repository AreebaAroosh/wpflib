using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Ksema.ObjectShell.WPF
{
    public class ValidationBindingHelper : DependencyObject
    {
        public static readonly DependencyProperty CollectErrorsProperty = DependencyProperty.RegisterAttached("CollectErrors", typeof(bool), typeof(ValidationBindingHelper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnCollectErrorsChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceCollectErrors) });

        public static void SetCollectErrors(FrameworkElement obj, bool value)
        {
            obj.SetValue(CollectErrorsProperty, value);
        }

        public static bool GetCollectErrors(FrameworkElement obj)
        {
            return (bool)obj.GetValue(CollectErrorsProperty);
        }

        #region OnCollectErrorsChanged
        private static void OnCollectErrorsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SetCollectError(o, (bool)e.NewValue);
        }
        #endregion

        #region OnCoerceCollectErrors
        private static object OnCoerceCollectErrors(DependencyObject o, object value)
        {
            return value;
        }
        #endregion

        public static readonly DependencyProperty CollectErrorProperty = DependencyProperty.RegisterAttached("CollectError", typeof(bool), typeof(ValidationBindingHelper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnCollectErrorChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceCollectError) });

        public static void SetCollectError(DependencyObject obj, bool value)
        {
            obj.SetValue(CollectErrorProperty, value);
        }

        public static bool GetCollectError(DependencyObject obj)
        {
            return (bool)obj.GetValue(CollectErrorProperty);
        }

        #region OnCollectErrorChanged
        private static void OnCollectErrorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && (bool)e.NewValue)
            {
                Validation.AddErrorHandler(o, OnValidationError);
            }
            else
            {
                Validation.RemoveErrorHandler(o, OnValidationError);
                o.SetValue(HasErrorPropertyKey, false);
                o.SetValue(ErrorsProperty, null);
            }
        }
        #endregion

        #region OnCoerceCollectError
        private static object OnCoerceCollectError(DependencyObject o, object value)
        {
            return value;
        }
        #endregion

        private static void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            var o = sender as DependencyObject;
            var collection = GetErrors(o);
            if (collection == null)
            {
                collection = new ObservableCollection<ValidationError>();
                SetErrors(o, collection);
            }
            switch (e.Action)
            {
                case ValidationErrorEventAction.Added:
                    collection.Add(e.Error);
                    break;
                case ValidationErrorEventAction.Removed:
                    collection.Remove(e.Error);
                    break;
                default:
                    break;
            }
            SetHasError(o, collection.Count > 0);
        }

        public static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasError", typeof(bool), typeof(ValidationBindingHelper), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        private static void SetHasError(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorPropertyKey, value);
        }

        public static bool GetHasError(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorProperty);
        }

        public static readonly DependencyPropertyKey ErrorsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Errors", typeof(ObservableCollection<ValidationError>), typeof(ValidationBindingHelper), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ErrorsProperty = ErrorsPropertyKey.DependencyProperty;

        private static void SetErrors(DependencyObject obj, ObservableCollection<ValidationError> value)
        {
            obj.SetValue(ErrorsPropertyKey, value);
        }

        public static ObservableCollection<ValidationError> GetErrors(DependencyObject obj)
        {
            return (ObservableCollection<ValidationError>)obj.GetValue(ErrorsProperty);
        }
    }
}

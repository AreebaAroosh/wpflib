using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFLib.DataWrapper;
using System.Collections.ObjectModel;

namespace WPFLib
{
    public class ValidationWrapper
    {
        static Dictionary<int, DependencyProperty> _internalValues;
        static Dictionary<int, DependencyProperty> InternalValues
        {
            get
            {
                if (_internalValues == null)
                {
                    _internalValues = new Dictionary<int, DependencyProperty>();
                }
                return _internalValues;
            }
        }
        static DependencyProperty GetInternalValueProperty(int index)
        {
            DependencyProperty value;
            if (!InternalValues.TryGetValue(index, out value))
            {
                value = DependencyProperty.RegisterAttached("InternalValue" + index.ToString(), typeof(object), typeof(ValidationWrapper), new FrameworkPropertyMetadata());
                InternalValues[index] = value;
            }
            return value;
        }

        public static readonly DependencyProperty ManagersProperty = DependencyProperty.RegisterAttached("Managers", typeof(ObservableCollection<object>), typeof(ValidationWrapper), new FrameworkPropertyMetadata());

        public static void SetManagers(DependencyObject obj, ObservableCollection<object> value)
        {
            obj.SetValue(ManagersProperty, value);
        }

        public static ObservableCollection<object> GetManagers(DependencyObject obj)
        {
            return (ObservableCollection<object>)obj.GetValue(ManagersProperty);
        }

        public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached("Id", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId(DependencyObject obj, string value) { obj.SetValue(IdProperty, value); }
        public static string GetId(DependencyObject obj) { return (string)obj.GetValue(IdProperty); }

        public static readonly DependencyProperty Id2Property = DependencyProperty.RegisterAttached("Id2", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId2(DependencyObject obj, string value) { obj.SetValue(Id2Property, value); }
        public static string GetId2(DependencyObject obj) { return (string)obj.GetValue(Id2Property); }

        public static readonly DependencyProperty Id3Property = DependencyProperty.RegisterAttached("Id3", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId3(DependencyObject obj, string value) { obj.SetValue(Id3Property, value); }
        public static string GetId3(DependencyObject obj) { return (string)obj.GetValue(Id3Property); }

        public static readonly DependencyProperty Id4Property = DependencyProperty.RegisterAttached("Id4", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId4(DependencyObject obj, string value) { obj.SetValue(Id4Property, value); }
        public static string GetId4(DependencyObject obj) { return (string)obj.GetValue(Id4Property); }

        public static readonly DependencyProperty Id5Property = DependencyProperty.RegisterAttached("Id5", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId5(DependencyObject obj, string value) { obj.SetValue(Id5Property, value); }
        public static string GetId5(DependencyObject obj) { return (string)obj.GetValue(Id5Property); }

        public static readonly DependencyProperty Id6Property = DependencyProperty.RegisterAttached("Id6", typeof(string), typeof(ValidationWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIdChanged) });
        public static void SetId6(DependencyObject obj, string value) { obj.SetValue(Id6Property, value); }
        public static string GetId6(DependencyObject obj) { return (string)obj.GetValue(Id6Property); }

        private static void OnIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (WPFHelper.IsInDesignMode)
                return;
            var indexString = e.Property.Name.Substring(2, e.Property.Name.Length - 2);
            int index;
            if (String.IsNullOrEmpty(indexString))
            {
                index = 1;
            }
            else
            {
                index = Int32.Parse(e.Property.Name.Substring(2, e.Property.Name.Length - 2));
            }
            InitializeManager(o, (string)e.NewValue, index);
        }

        static void InitializeManager(DependencyObject target, string id, int index)
        {
            var manager = new ValidationWrapperAttachedManager(id, (FrameworkElement)target, GetInternalValueProperty(index));
            AddManager(target, manager);
        }

        static void AddManager(DependencyObject o, object manager)
        {
            var managers = GetManagers(o);
            if (managers == null)
            {
                managers = new ObservableCollection<object>();
                SetManagers(o, managers);
            }
            managers.Add(manager);
        }
    }
}

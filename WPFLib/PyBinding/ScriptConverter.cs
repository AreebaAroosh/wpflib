using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;

namespace PyBinding
{
    [MarkupExtensionReturnType(typeof(object))]
    public class ScriptConverter : IValueConverter, IMultiValueConverter
    {
        #region Constructors

        static ScriptConverter()
        {
            PythonEvaluator = new PythonEvaluator();
            DefaultValues = new Dictionary<Type, object>
                                {
                                    {typeof (int), 0},
                                    {typeof (string), null},
                                    {typeof (double), 0.0D},
                                    {typeof (decimal), 0.0M},
                                    {typeof (bool), false},
                                    {typeof (float), 0.0F},
                                    {typeof (long), 0L},
                                    {typeof (short), 0}
                                };
        }

        #endregion

        #region Properties

        private static PythonEvaluator PythonEvaluator { get; set; }

        private static IDictionary<Type, object> DefaultValues { get; set; }

        #endregion

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new[] { value }, targetType, parameter, culture);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            try
            {
                var pyBinding = parameter as IPyBinding;
                if (pyBinding == null || (pyBinding.UnsetValueIsInvalid && ContainsUnsetValue(values)))
                    return GetDefaultValue(targetType);

                var result = PythonEvaluator.ExecuteWithResult(pyBinding.CompiledCode, values);
                return result ?? GetDefaultValue(targetType);
            }
            catch (Exception)
            {
                return GetDefaultValue(targetType);
            }
        }

        private static bool ContainsUnsetValue(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i] == DependencyProperty.UnsetValue)
                    return true;
            
            return false;
        }

        private static object GetDefaultValue(Type targetType)
        {
            if (DefaultValues.ContainsKey(targetType))
                return DefaultValues[targetType];
            
            object result = null;

            if (targetType.IsValueType)
                result = Activator.CreateInstance(targetType);

            DefaultValues.Add(targetType, result);
            
            return result;
        }       

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("PyBinding does not support two way binding");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("PyBinding does not support two way binding");
        }

        #endregion
    }
}

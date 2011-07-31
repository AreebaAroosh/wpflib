using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WPFLib
{
    public static class BindingHelper
    {
        public static Binding Clone(this Binding b)
        {
            var n = new Binding();

            n.Path = b.Path;
            n.Mode = b.Mode;
            n.BindingGroupName = b.BindingGroupName;
            if(n.BindsDirectlyToSource != b.BindsDirectlyToSource)
                n.BindsDirectlyToSource = b.BindsDirectlyToSource;
            n.Converter = b.Converter;
            n.ConverterCulture = b.ConverterCulture;
            n.ConverterParameter = b.ConverterParameter;

            if(n.ElementName != b.ElementName)
                n.ElementName = b.ElementName;
            if(n.FallbackValue != b.FallbackValue)
                n.FallbackValue = b.FallbackValue;
            n.IsAsync = b.IsAsync;
            if (b.Source != null)
                n.Source = b.Source;
            if (b.RelativeSource != null)
                n.RelativeSource = b.RelativeSource;
            n.StringFormat = b.StringFormat;
            n.TargetNullValue = b.TargetNullValue;
            n.UpdateSourceExceptionFilter = b.UpdateSourceExceptionFilter;
            n.UpdateSourceTrigger = b.UpdateSourceTrigger;
            n.ValidatesOnDataErrors = b.ValidatesOnDataErrors;
            n.ValidatesOnExceptions = b.ValidatesOnExceptions;
            foreach (var r in b.ValidationRules)
                n.ValidationRules.Add(r);
            n.XPath = b.XPath;

            return n;
        }
    }
}

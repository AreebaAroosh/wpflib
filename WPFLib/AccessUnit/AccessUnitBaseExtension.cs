using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WPFLib.MarkupExtensionHelpers;

namespace WPFLib.AccessUnit
{
    public abstract class AccessUnitBaseExtension : MarkupExtensionBase
    {
        string UnitName;
        public AccessUnitBaseExtension(string unitName)
        {
            UnitName = unitName;
        }

        protected abstract IValueConverter ProvideConverter();

        protected override object ProvideValue(IServiceProvider serviceProvider, System.Windows.FrameworkElement targetElement, System.Windows.DependencyProperty targetProperty)
        {
            var manager = new AccessUnitExtensionAttachedManager(UnitName, ProvideConverter(), targetElement, targetProperty);

            var metaData = targetProperty.GetMetadata(targetElement);
            return metaData.DefaultValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using WPFLib.AccessUnit;
using WPFLib.MarkupExtensionHelpers;

namespace WPFLib.AccessUnit
{
    public sealed class AccessUnitExtensionAttachedManager : MarkupExtensionDataContextTracker
    {
        string UnitName;
        IValueConverter Converter;

        public AccessUnitExtensionAttachedManager(string unitName, IValueConverter converter, FrameworkElement target, DependencyProperty property)
            : base(target, property)
        {
            UnitName = unitName;
            Converter = converter;
            InitialDataContextCheck();
        }

        protected override void OnTargetDataContextChanged()
        {
            var provider = TargetElement.DataContext as IAccessUnitModeProvider;
            if (provider != null)
            {
                var unit = provider.GetUnit(UnitName);
                var b = new Binding("Mode") { Source = unit, Mode = BindingMode.OneWay };

                b.Converter = Converter;

                BindingOperations.SetBinding(TargetElement, TargetProperty, b);
            }
            else
            {
                BindingOperations.ClearBinding(TargetElement, TargetProperty);
            }
        }
    }
}

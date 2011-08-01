using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPFLib
{
    /// <summary>
    //<UserControl.Resources>
    //    <EnumDisplayer x:Key="AccessUnitMode" Type="{x:Type au:AccessUnitMode}"></EnumDisplayer>
    //</UserControl.Resources>
    //<ComboBox ItemsSource="{Binding Source={StaticResource AccessUnitMode}, Path=DisplayNames}" SelectedValue="{Binding UserNameWrapper.Mode,Converter={StaticResource AccessUnitMode}}"/>
    /// </summary>
    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDisplayer : IValueConverter
    {
        private Type type;
        private IDictionary displayValues;
        private IDictionary reverseValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;

        public EnumDisplayer()
        {
        }

        public EnumDisplayer(Type type)
        {
            this.Type = type;
        }

        public Type Type
        {
            get { return type; }
            set
            {
                if (!value.IsEnum)
                    throw new ArgumentException("parameter is not an Enumermated type", "value");
                this.type = value;
            }
        }


        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                Type displayValuesType = typeof(Dictionary<,>)
                                            .GetGenericTypeDefinition().MakeGenericType(typeof(string), type);
                this.displayValues = (IDictionary)Activator.CreateInstance(displayValuesType);

                this.reverseValues =
                   (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                            .GetGenericTypeDefinition()
                            .MakeGenericType(type, typeof(string)));

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields)
                {
                    DisplayStringAttribute[] a = (DisplayStringAttribute[])
                                                field.GetCustomAttributes(typeof(DisplayStringAttribute), false);

                    string displayString = GetDisplayStringValue(a);
                    object enumValue = field.GetValue(null);

                    if (displayString == null)
                    {
                        displayString = GetBackupDisplayStringValue(enumValue);
                    }
                    if (displayString != null)
                    {
                         reverseValues.Add(enumValue, displayString);
                         displayValues.Add(displayString, enumValue);
                    }
                }
                return new List<string>((IEnumerable<string>)reverseValues.Values).AsReadOnly();
            }
        }

        private string GetDisplayStringValue(DisplayStringAttribute[] a)
        {
            if (a == null || a.Length == 0) return null;
            DisplayStringAttribute dsa = a[0];
            if (!string.IsNullOrEmpty(dsa.ResourceKey))
            {
                ResourceManager rm = new ResourceManager(type);
                return rm.GetString(dsa.ResourceKey);
            }
            return dsa.Value;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (overriddenDisplayEntries != null && overriddenDisplayEntries.Count > 0)
            {
                EnumDisplayEntry foundEntry = overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
                                                 {
                                                     object e = Enum.Parse(type, entry.EnumValue);
                                                     return enumValue.Equals(e);
                                                 });
                if (foundEntry != null)
                {
                    if (foundEntry.ExcludeFromDisplay) return null;
                    return foundEntry.DisplayString;

                }
            }
            return Enum.GetName(type, enumValue);
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (overriddenDisplayEntries == null)
                    overriddenDisplayEntries = new List<EnumDisplayEntry>();
                return overriddenDisplayEntries;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (reverseValues==null || reverseValues.Count==0)
			{
				var dummy = DisplayNames;
			}
        	return  reverseValues[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (displayValues == null || displayValues.Count == 0)
			{
				var dummy = DisplayNames;
			}
            return displayValues[value];
        }
    }

    public class EnumDisplayEntry
    {
        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisplayStringAttribute : Attribute
    {
        private readonly string value;
        public string Value
        {
            get { return value; }
        }

        public string ResourceKey { get; set; }

        public DisplayStringAttribute(string v)
        {
            this.value = v;
        }

        public DisplayStringAttribute()
        {
        }
    }
}

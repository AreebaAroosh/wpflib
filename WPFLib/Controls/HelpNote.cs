using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace WPFLib
{
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class HelpNote : Control
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(HelpNote), new FrameworkPropertyMetadata());
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        static HelpNote()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HelpNote), new FrameworkPropertyMetadata(typeof(HelpNote)));
        }
    }
}

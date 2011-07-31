using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Sgml;
using System.IO;
using System.Windows.Documents;
using WPFLib.Navigation;
using WPFLib.Hyperlinks;

namespace WPFLib
{
    public class HyperLinkTextBlock : TextBlock
    {
        public static readonly DependencyProperty HyperLinkTextProperty = DependencyProperty.Register("HyperLinkText", typeof(string), typeof(HyperLinkTextBlock), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnHyperLinkTextChanged)});
        public string HyperLinkText
        {
            get { return (string)GetValue(HyperLinkTextProperty); }
            set { SetValue(HyperLinkTextProperty, value); }
        }

        #region OnHyperLinkTextChanged
        private static void OnHyperLinkTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HyperLinkTextBlock control = o as HyperLinkTextBlock;
            if (control != null)
                control.OnHyperLinkTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        public HyperLinkTextBlock()
        {
            AddHandler(Hyperlink.ClickEvent, (RoutedEventHandler)Hyperlink_Click);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Hyperlink)
            {
                var uri = ((e.OriginalSource as Hyperlink).NavigateUri);
                if (uri != null)
                {
                    var path = uri.LocalPath.TrimStart('/');
                    if (NavigationManager.IsNavigationManagerAvailable)
                    {
                        NavigationManager.Instance.Open(path);
                    }
                }
                e.Handled = true;
            }
        }

        private void OnHyperLinkTextChanged(string oldValue, string newValue)
        {
            this.Inlines.Clear();
            foreach (var i in HyperLinkHelper.Convert(newValue))
                this.Inlines.Add(i);
        }
    }
}

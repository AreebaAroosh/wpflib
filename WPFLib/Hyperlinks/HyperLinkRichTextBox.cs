using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using WPFLib.Navigation;
using WPFLib.Hyperlinks;

namespace WPFLib
{
    public class HyperLinkRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty HyperLinkTextProperty = DependencyProperty.Register("HyperLinkText", typeof(string), typeof(HyperLinkRichTextBox), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnHyperLinkTextChanged) });
        public string HyperLinkText
        {
            get { return (string)GetValue(HyperLinkTextProperty); }
            set { SetValue(HyperLinkTextProperty, value); }
        }

        #region OnHyperLinkTextChanged
        private static void OnHyperLinkTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HyperLinkRichTextBox control = o as HyperLinkRichTextBox;
            if (control != null)
                control.OnHyperLinkTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        public HyperLinkRichTextBox()
        {
            this.IsDocumentEnabled = true;
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
            }
        }

        private void OnHyperLinkTextChanged(string oldValue, string newValue)
        {
            //Debug.WriteLine("Start build");
            this.Document.Blocks.Clear();
            var par = new Paragraph();

            foreach (var i in HyperLinkHelper.Convert(newValue))
                par.Inlines.Add(i);

            this.Document.Blocks.Add(par);
            //Debug.WriteLine("End build");
        }

    }
}

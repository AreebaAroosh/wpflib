using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Interop;
using WPFLib.Misc;

namespace WPFLib.Dialogs
{
    /// <summary>
    /// Красивое окно с текстом на замену MessageBox'у
    /// </summary>
    public partial class DetailedMessageBox : Window, IDisposable
    {
        const int DETAILS_HEIGHT = 250;

        public DetailedMessageBox()
        {
            InitializeComponent();

            {
                var closeCommand = new CommandBinding(ApplicationCommands.Close);
                closeCommand.Executed +=new ExecutedRoutedEventHandler(closeCommand_Executed);
                this.CommandBindings.Add(closeCommand);
            }

            this.Loaded += new RoutedEventHandler(DetailedMessageBox_Loaded);
            //pictureBoxInfo.Source = convertBitmapToBitmapSource(SystemIcons.Information.ToBitmap());
        }

        void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        bool isInitialized = false;

        protected override void OnActivated(EventArgs e)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ClearValue(SizeToContentProperty);
                ClearValue(WidthProperty);
                ClearValue(HeightProperty);
            }
            base.OnActivated(e);
        }

        void DetailedMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.WorkArea.Height;

            var helper = new WindowInteropHelper(this);
            if (helper.Owner != IntPtr.Zero)
            {
                Flasher.FlashWindowEx(helper.Owner);
            }
            else
            {
                // Без SizeToContent окно плюнет на это изменение
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                this.ShowInTaskbar = true;
                Flasher.FlashWindowEx(helper.Handle);
            }
        }

        public void Init(string message, string details)
        {
			Init(message, details, false);
        }

        public void Init(string message, string details, bool showSendLogs)
        {
            Init("Information", message, details, showSendLogs);
        }

        public void Init(string title, string message, string details)
        {
			Init(title, message, details, false);
        }

        public void Init(string title, string message, string details, bool showSendLogs)
        {
            Title = title;
            Message = message;
            Details = details;
            ShowSendLogs = false;// showSendLogs && Reporter != null;
        }

		public static readonly DependencyProperty ShowSendLogsProperty = DependencyProperty.Register("ShowSendLogs", typeof(bool), typeof(DetailedMessageBox), new UIPropertyMetadata());
		public bool ShowSendLogs
        {
            get { return (bool)GetValue(ShowSendLogsProperty); }
            set { SetValue(ShowSendLogsProperty, value); }
        }

		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(DetailedMessageBox), new UIPropertyMetadata());
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register("Details", typeof(string), typeof(DetailedMessageBox), new UIPropertyMetadata());
        public string Details
        {
            get { return (string)GetValue(DetailsProperty); }
            set { SetValue(DetailsProperty, value); }
        }

        public static readonly DependencyProperty IsDetailedProperty = DependencyProperty.Register("IsDetailed", typeof(bool), typeof(DetailedMessageBox), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnIsDetailedChanged) });
        public bool IsDetailed
        {
            get { return (bool)GetValue(IsDetailedProperty); }
            set { SetValue(IsDetailedProperty, value); }
        }

        #region OnIsDetailedChanged
        private static void OnIsDetailedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DetailedMessageBox control = o as DetailedMessageBox;
            if (control != null)
                control.OnIsDetailedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsDetailedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                msgRow.Height = new GridLength(1);
                detailsRow.Height = new GridLength(1, GridUnitType.Star);
                this.Height = this.MinHeight + DETAILS_HEIGHT;
                this.MinHeight += DETAILS_HEIGHT;
            }
            else
            {
                msgRow.Height = new GridLength(1, GridUnitType.Star);
                detailsRow.Height = new GridLength(1);
                this.MinHeight -= DETAILS_HEIGHT;
                this.Height = this.MinHeight;
            }
        }
        #endregion

        public ImageSource Image
        {
            get { return pictureBoxInfo.Source; }
            set { pictureBoxInfo.Source = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        //[DebuggerNonUserCode]
        //private IErrorReporter Reporter
        //{
        //    get
        //    {
        //        try
        //        {
        //            return ObjectContainer.Instance.Resolve<IErrorReporter>();
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
            //var reporter = Reporter;
            //if (reporter != null)
            //    reporter.SendReport(Message, Details);
		}
    }
}

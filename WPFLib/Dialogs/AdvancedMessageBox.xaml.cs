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

namespace WPFLib.Dialogs
{
	/// <summary>
	/// Interaction logic for AdvancedMessageBox.xaml
	/// </summary>
	public partial class AdvancedMessageBox : Window
	{
		public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons)
		{
			AdvancedMessageBox box = new AdvancedMessageBox(text, caption, buttons);
			box.ShowOwnedDialog();
			return box.Result;
		}

		public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage image)
		{
			AdvancedMessageBox box = new AdvancedMessageBox(Application.Current.MainWindow, text, caption, buttons, image);
			box.ShowOwnedDialog();
			return box.Result;
		}

		public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons)
		{
			AdvancedMessageBox box = new AdvancedMessageBox(owner, text, caption, buttons);
			box.ShowOwnedDialog();
			return box.Result;
		}


		public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons, MessageBoxImage image)
		{
			AdvancedMessageBox box = new AdvancedMessageBox(owner,text, caption, buttons, image);
			box.ShowOwnedDialog();
			return box.Result;
		}


		public AdvancedMessageBox( string text, string caption, MessageBoxButton buttons):this(Application.Current.MainWindow, text, caption, buttons )
		{
		}

		public AdvancedMessageBox(Window owner, string text, string caption, MessageBoxButton buttons):this(owner, text, caption, buttons, MessageBoxImage.Warning)
		{
		}


		public AdvancedMessageBox(Window owner, string text, string caption, MessageBoxButton buttons, MessageBoxImage image)
		{
			InitializeComponent();
			Owner = owner;
			txtMain.AppendText(text);
			Title = caption;
			SetVisibleButtons(buttons);
			ImageType = image;
			Result = MessageBoxResult.None;
		}



		public MessageBoxImage ImageType
		{
			get { return (MessageBoxImage)GetValue(ImageTypeProperty); }
			set { SetValue(ImageTypeProperty, value); }
		}
		
		public static readonly DependencyProperty ImageTypeProperty =
			DependencyProperty.Register("ImageType", typeof(MessageBoxImage), typeof(AdvancedMessageBox), new UIPropertyMetadata(MessageBoxImage.Warning));

		

		private void SetVisibleButtons(MessageBoxButton button)
		{
			switch (button)
			{
				case MessageBoxButton.OK:
					btnOK.Visibility = Visibility.Visible;
					break;
				case MessageBoxButton.OKCancel:
					btnOK.Visibility=Visibility.Visible;
					btnCancel.Visibility = Visibility.Visible;
					break;
				case MessageBoxButton.YesNoCancel:
					btnYes.Visibility = Visibility.Visible;
					btnNo.Visibility = System.Windows.Visibility.Visible;
					btnCancel.Visibility = Visibility.Visible;
					break;
				case MessageBoxButton.YesNo:
					btnYes.Visibility =Visibility.Visible;
					btnNo.Visibility=Visibility.Visible;
					break;
				default:
					throw new ArgumentOutOfRangeException("button");
			}
		}

		private MessageBoxResult Result { get; set; }

		private void OKClick(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.OK;
			Close();
		}

		private void YesClick(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			Close();
		}

		private void NoClick(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.No;
			Close();
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Cancel;
			Close();
		}
	}
}

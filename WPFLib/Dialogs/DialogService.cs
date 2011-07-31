using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using WPFLib.Contracts;
using System.ComponentModel.Composition;
using WPFLib.Misc;

namespace WPFLib.Dialogs
{
	[Export(typeof(IDialogService))]
	internal class DialogService : DispatcherPropertyChangedHelper, IDialogService
	{
		public MessageBoxResult Ask(string question, string title, bool showCancel = true)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				MessageBoxResult result = MessageBoxResult.No;
				Application.Current.Dispatcher.Invoke(() => result = Ask(question, title, showCancel));
				return result;
			}
			else
				return AdvancedMessageBox.Show(question, title, showCancel ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo, MessageBoxImage.Question);
		}

		public void Message(string title, string msg, string details)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => Message(title, msg, details));
				return;
			}
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init(title, msg, details, false);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void Message(string msg)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => Message(msg));
				return;
			}
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init("Сообщение", msg, msg);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void Message(string msg, string title)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => Message( msg, title));
				return;
			}
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init(title, msg, msg);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void ErrorMessage(Exception ex)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => ErrorMessage(ex));
				return;
			}
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init("Ошибка в приложении", ex.ShortExceptionMessage(), ex.FullExceptionMessage(), true);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void ErrorMessage(string msg, Exception ex)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => ErrorMessage(msg, ex));
				return;
			}
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init("Ошибка в приложении", msg, ex.FullExceptionMessage(), true);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void ErrorMessage(string msg)
		{
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init("Ошибка в приложении", msg, msg, true);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void ErrorMessage(string msg, string details)
		{
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init("Ошибка в приложении", msg, details, true);
				ShowDetailedMessageBox(dlg);
			}
		}

		public void SevereErrorMessage(string title, string msg, string details)
		{
			using (DetailedMessageBox dlg = new DetailedMessageBox())
			{
				dlg.Init(title, msg, details);
				dlg.ShowInTaskbar = true;
				dlg.ShowOwnedDialog();
			}
		}

		public void ShowDetailedMessageBox(DetailedMessageBox messageBox)
		{
			//var owner = GetWindowOwner();
			//if (owner == null)
			//    messageBox.ShowInTaskbar = true;
			messageBox.ShowOwnedDialog();
		}
	}
}

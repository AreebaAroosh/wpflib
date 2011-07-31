using System;
using System.Windows;

namespace WPFLib.Contracts
{
    public interface IDialogService
    {
        MessageBoxResult Ask(string question, string title, bool showCancel = true);
        void ErrorMessage(Exception ex);
        void ErrorMessage(string msg);
        void ErrorMessage(string msg, Exception ex);
        void ErrorMessage(string msg, string details);
		void Message(string title, string msg, string details);
        void Message(string msg);
        void Message(string msg, string title);
        void SevereErrorMessage(string title, string msg, string details);
    }
}

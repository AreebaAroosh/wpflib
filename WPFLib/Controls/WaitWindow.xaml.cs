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
using System.Threading.Tasks;
using System.Threading;

namespace WPFLib.Controls
{
	/// <summary>
	/// Interaction logic for WaitWindow.xaml
	/// </summary>
	public partial class WaitWindow : Window
	{
		public static Task Wait(Action action)
		{
			return Wait(Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default), null);
		}

		public static Task Wait(Action action, Action<Exception> errorAction)
		{
			return Wait(Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default), errorAction);
		}

		bool closing = false;
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !closing;
		}

		public void ForceClose()
		{
			closing = true;
			this.Close();
		}

		public void SetText(string text)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(() => SetText(text));
			else
				runText.Text = text;
		}

		public static Task Wait(Task task, Action<Exception> errorAction)
		{
			var w = new WaitWindow();
			//if (Application.Current != null && Application.Current.MainWindow != null)
			//{
			//    w.Owner = Application.Current.MainWindow;
			//}
			if (errorAction != null)
			{
				task.ContinueWith((t) => {
					w.Dispatcher.BeginInvoke(w.ForceClose);
					errorAction(t.Exception);
				}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
			}
			task.ContinueWith((t) => w.Dispatcher.BeginInvoke(w.ForceClose));
			w.ShowOwnedDialog();
			return task;
		}

		public WaitWindow()
		{
			InitializeComponent();
		}
	}
}

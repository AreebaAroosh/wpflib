using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows;
using WPFLib.Contracts;

namespace WPFLib.MainWindow
{
	[Export(typeof(IApplication))]
	public class AppMgr : IApplication
	{
		public bool UseDispatcher
		{
			get;
			set;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Contracts
{
	public interface IApplication
	{
		bool UseDispatcher
		{
			get;
			set;
		}
	}
}

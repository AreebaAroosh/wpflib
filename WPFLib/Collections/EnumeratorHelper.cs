using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Collections
{
	public static class EnumeratorHelper
	{
		public static IEnumerable<T> AsEnumerable<T>(this T item)
		{
			yield return item;
		}
	}
}

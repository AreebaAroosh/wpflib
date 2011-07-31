using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Misc
{
	public static class ExceptionHelper
	{
		public static string ShortExceptionMessage(this Exception e)
		{
			return GetExceptionText(e, i => i.Message);
		}

		public static string FullExceptionMessage(this Exception e)
		{
			return GetExceptionText(e, i => i.ToString());
		}

		private static string GetExceptionText(Exception e, Func<Exception, string> toString)
		{
			StringBuilder s = new StringBuilder(toString(e));
			while ((e = e.InnerException) != null)
				s.AppendLine().Append(toString(e));
			return s.ToString();
		}
	}
}

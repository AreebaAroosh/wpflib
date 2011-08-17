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

			//*
			GetExceptionTextImpl(e, toString, s);
			/*/
			while ((e = e.InnerException) != null)
				s.AppendLine().Append(toString(e));
			//*/

			return s.ToString();
		}

		private static void GetExceptionTextImpl(Exception e, Func<Exception, string> toString, StringBuilder s)
		{
			if (e is AggregateException)
			{
				foreach (var inner in (e as AggregateException).InnerExceptions)
					GetExceptionTextImpl(inner, toString, s);
			}
			//else
			{
				var inner = e.InnerException;
				if (inner != null)
				{
					s.AppendLine().Append(toString(inner));
					GetExceptionTextImpl(inner, toString, s);
				}
			}
		}
	}
}

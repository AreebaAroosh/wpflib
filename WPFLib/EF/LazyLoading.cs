using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Context;

namespace WPFLib.EF
{
	public class LazyLoading : IDisposable
	{
		private readonly IContext _context;
		private readonly bool _oldLazyLoading;

		public LazyLoading(IContext context) : this(context, true)
		{
		}

		public LazyLoading(IContext context, bool enable)
		{
			_context = context;
			_oldLazyLoading = _context.ContextOptions.LazyLoadingEnabled;
			_context.ContextOptions.LazyLoadingEnabled = enable;
		}

		public void Dispose()
		{
			_context.ContextOptions.LazyLoadingEnabled = _oldLazyLoading;
		}
	}
}

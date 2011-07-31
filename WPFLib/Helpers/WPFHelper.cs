using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace WPFLib
{
	public static class WPFHelper
	{
		private static bool? _isInDesignMode;
		/// <summary>
		/// Gets a value indicating whether the control is in design mode (running in Blend
		/// or Visual Studio).
		/// </summary>
		public static bool IsInDesignMode
		{
			get
			{
				if( !_isInDesignMode.HasValue )
				{
#if SILVERLIGHT
            _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					_isInDesignMode = DesignerProperties.GetIsInDesignMode( new DependencyObject() );
#endif
				}
				return _isInDesignMode.Value;
			}
		}

		private static Action EmptyDelegate = delegate(){};

		public static void Refresh( this UIElement uiElement )
		{
			uiElement.Dispatcher.Invoke( DispatcherPriority.Render, EmptyDelegate );
		}
	}
}

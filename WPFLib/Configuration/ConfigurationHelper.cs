using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Configuration;

namespace System
{
    public static class ConfigurationHelper
    {
        public static T Resolve<T>(this object o)
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

		public static T Resolve<T>(this object o, string name)
		{
			return ServiceLocator.Current.GetInstance<T>(name);
		}
    }
}

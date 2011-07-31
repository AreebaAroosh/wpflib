using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.DataWrapper
{
    public interface IDataWrapperProvider
    {
        IDataWrapper GetDataWrapper(string property);
    }

	public interface IDataWrapperInitializer
	{
		void InitializeWrappers();
	}

    public interface IValidationWrapperProvider
    {
        string DefaultUri { get; }
        IValidationWrapper GetValidationWrapper(string id);
    }
}

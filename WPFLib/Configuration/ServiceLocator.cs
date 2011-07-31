using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel.Composition;

namespace WPFLib.Configuration
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use ServiceLocator.Current
    /// to get it.
    /// </summary>
    public static class ServiceLocator
    {
        // Напрямую привяжем MEF, надо подумать
        private static ServiceLocatorProvider currentProvider = MefServiceLocatorProvider.Get;

        /// <summary>
        /// The current ambient container.
        /// </summary>

        public static IServiceLocator Current
        {
            get { return currentProvider(); }
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the current container.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return
        /// the current ambient container.</param>
        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            currentProvider = newProvider;
        }
    }
}

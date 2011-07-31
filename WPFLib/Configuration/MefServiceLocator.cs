using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;

namespace Microsoft.Mef.CommonServiceLocator
{
	public class MefServiceLocator : ServiceLocatorImplBase
	{
        bool initialized = false;
        Task initializationTask;

        public override void Initialize()
        {
            if(initialized || initializationTask != null)
                return;
            initializationTask = Task.Factory.StartNew(() => {
                provider.GetExports<IComparable>();
                initialized = true;
                initializationTask = null;
            });
        }

        public override IEnumerable<Lazy<T, M>> GetExports<T, M>(string contractName)
        {
            try
            {
                return provider.GetExports<T, M>(contractName);
            }
            finally
            {
                ClearDisposableTrackingHack();
            }
        }

		private ExportProvider provider;

		public MefServiceLocator(ExportProvider provider)
		{
			this.provider = provider;
		}

        HashSet<IDisposable> disposableTracker = null;
        private void ClearDisposableTrackingHack()
        {
            if (disposableTracker == null)
            {
                var exportProvider = provider.GetType().GetField("_catalogExportProvider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var exportProviderVal = (CatalogExportProvider)exportProvider.GetValue(provider);

                var disposableTrackerField = exportProviderVal.GetType().GetField("_partsToDispose", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                disposableTracker = (HashSet<IDisposable>)disposableTrackerField.GetValue(exportProviderVal);
            }
            disposableTracker.Clear();
        }

		protected override object DoGetInstance(Type serviceType, string key)
		{
            try
            {
                if (key == null)
                {
                    key = AttributedModelServices.GetContractName(serviceType);
                }

                if (initializationTask != null)
                    initializationTask.Wait();
                IEnumerable<Lazy<object>> exports = provider.GetExports<object>(key);

                if (exports.Any())
                {
                    return exports.First().Value;
                }

            }
            finally
            {
                ClearDisposableTrackingHack();
            }
			throw new ActivationException(string.Format("Could not locate any instances of contract {0}", key));
		}

		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
            try {
                if (initializationTask != null)
                    initializationTask.Wait();
			    var exports = provider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
			    return exports;
            }
            finally
            {
                ClearDisposableTrackingHack();
            }
		}
	}
}
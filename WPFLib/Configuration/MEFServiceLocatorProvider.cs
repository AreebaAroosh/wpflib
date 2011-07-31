using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Mef.CommonServiceLocator;
using System.ComponentModel.Composition.Hosting;

namespace WPFLib.Configuration
{
    public static class MefServiceLocatorProvider
    {
        static string _assemblyLoadPattern;
        static bool isPatternSet = false;
        public static string AssemblyLoadPattern
        {
            get
            {
                return _assemblyLoadPattern;
            }
            set
            {
                if (isPatternSet)
                    throw new Exception("AssemblyLoadPattern already set");
                _assemblyLoadPattern = value;
            }
        }

        static MefServiceLocatorProvider()
        {
            ServiceLocator.SetLocatorProvider(Get);
        }

        static IServiceLocator Initialize()
        {
            var catalog = new AggregateCatalog();
            if (String.IsNullOrEmpty(AssemblyLoadPattern))
            {
                catalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));
            }
            else
            {
                catalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, AssemblyLoadPattern));
            }
            var entry = System.Reflection.Assembly.GetEntryAssembly();
            if(entry != null)
                catalog.Catalogs.Add(new AssemblyCatalog(entry));
            catalog.Catalogs.Add(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()));

            var container = new CompositionContainer(catalog);
            return new MefServiceLocator(container);
        }

        static IServiceLocator current = null;

        public static IServiceLocator Get()
        {
            if (current == null)
            {
                current = Initialize();
            }
            return current;
        }
    }
}

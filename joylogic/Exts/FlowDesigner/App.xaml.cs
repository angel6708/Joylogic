using Core;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FlowDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ////Add PRIVATE_BINPATH
            //AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "Modules;");
            //AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", "Modules;");
            //var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            //var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            //m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", "Modules;" });

            //foreach (var file in  Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"),"*.dll") ) 
            //{
            //    var assemblyName = Path.GetFileNameWithoutExtension(file);
            //    var assembly = AppDomain.CurrentDomain.Load(assemblyName);


            //    var type = assembly.GetTypes().Where(x => typeof(IModule).IsAssignableFrom(x)).FirstOrDefault();
            //    if (type != null)
            //    {
            //        var module = ContainerService.Current.GetInstance(type) as IModule;
            //        module.Initialize();
            //    }
            //}


            base.OnStartup(e);

#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInDebugMode()
        {

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                Bootstrapper bootstrapper = new Bootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;
            //log 
            Environment.Exit(1);
        }
    }
}

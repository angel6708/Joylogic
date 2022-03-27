using Core.Infrastructure.Logging;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Joylogic
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>>
    public partial class App : Application
    {
        public static ResourceDictionary _ClientViewResource { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        { 
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            /*
            Debug.Listeners.Clear();
            if (!Directory.Exists("Log"))
            {
                Directory.CreateDirectory("Log");
            }
            Debug.Listeners.Add(new TextWriterTraceListener("Log\\" + DateTime.Now.ToString("yyyy_MM_dd") + ".Debug.log"));
            */

            //ResourceDictionary localResource = this.Resources;
            //_ClientViewResource = new ResourceDictionary()
            //{
            //    Source = new Uri("pack://application:,,,/Core.Resources;component/ListViewStyle.xaml"),
            //};
            // localResource.MergedDictionaries.Add(_ClientViewResource);

            base.OnStartup(e);
            //  System.Debug.WriteLine(DEBUG);

#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        /// <summary>
        /// ListView资源字典根据客户端运行而加载
        /// </summary>
        /// <param name="isClient"></param>
        public void InitSelectChanged(bool isClient)
        {
            ResourceDictionary localResource = this.Resources;


            if (!localResource.MergedDictionaries.Contains(_ClientViewResource))
            {
                localResource.MergedDictionaries.Add(_ClientViewResource);
            }


        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

        }

        private static void RunInDebugMode()
        {
           // AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
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
            try
            {
                ContainerService.Current.GetInstance<ILoggerFacade>().Log(ex.StackTrace, Category.Exception, Priority.High);
                MessageBox.Show("发现没有处理的异常,请查看日志,程序退出");
            }
            catch
            {
                MessageBox.Show(ex.StackTrace);
            }

            Environment.Exit(1);
        }

    }
}

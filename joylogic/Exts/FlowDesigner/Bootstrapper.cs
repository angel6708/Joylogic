using System.Windows;
using Core.Infrastructure.ModuleLoader;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
 
  
using FlowDesigner;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Services;
using Core.Infrastructure.Services.ext;
using Prism.Modularity;
using Unity;
using System.Windows.Threading;
using Core.Infrastructure.Utils;
using Prism.Unity;
using Prism.Regions;
using Prism.Ioc;
using Core.Infrastructure.Logging;

namespace Core
{


    public partial class Bootstrapper : PrismBootstrapper
    {

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            // string s = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

            base.ConfigureModuleCatalog(moduleCatalog);
            DirectoryModuleCatalog directoryCatalog = null;

            //directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Data" };
            //((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);


            // These modules are not referenced in the project and are discovered by inspecting a directory.
            // Both projects have a post-build step to copy themselves into that directory.
            directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
            ((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);


            //directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\LastModule" };
            //((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);



        }


        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new AggregateModuleCatalog();
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            base.InitializeShell(shell);
            Application.Current.MainWindow = (MainWindow)this.Shell;
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            Application.Current.MainWindow.Show();
        }


        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(NavigationPanel), this.Container.Resolve<NavigationPanelRegionAdapter>());

            }
        }



        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<IUnityContainer>())
            {
                containerRegistry.RegisterSingleton(typeof(IUnityContainer), typeof(UnityContainer));
                containerRegistry.RegisterInstance<IUnityContainer>(this.Container.GetContainer());

            }
             

            ContainerService.Current.SetContainer(this.Container.GetContainer());
            ContainerService.Current.Configure();
            ContainerService.Current.RegisterInstance<ILoggerFacade>(new Log4NetLoggerAdaptor());


        }

        protected override DependencyObject CreateShell()
        {
            UIDispatcher.Dispatcher = Dispatcher.CurrentDispatcher;
            return Container.Resolve<MainWindow>();
        }
    }

}

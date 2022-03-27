using System.Windows;
using Core.Infrastructure; 
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Core.Infrastructure.Services;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Logging;
using Core.Infrastructure.ModuleLoader;
using Core.Infrastructure.Services.ext;
using Joylogic.Views;
using System;
using Core.Infrastructure.Utils;
using System.Windows.Threading;
using Prism.Modularity;
using Prism.Unity;
using Prism;
using Prism.Ioc;
using Unity;
using Prism.Regions;
using Core.Infrastructure.Workflow;

namespace Joylogic
{

    public partial class Bootstrapper : PrismBootstrapper
    {

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

           // string s = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

            base.ConfigureModuleCatalog(moduleCatalog);
            DirectoryModuleCatalog directoryCatalog = null;

            directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Data" };
            ((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);


            // These modules are not referenced in the project and are discovered by inspecting a directory.
            // Both projects have a post-build step to copy themselves into that directory.
            directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
            ((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);


            directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\LastModule" };
            ((AggregateModuleCatalog)moduleCatalog).AddCatalog(directoryCatalog);


             
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

            //create ToolBarView
            var view = ContainerService.Current.CreateInstance<ToolBarView>();
            ContainerService.Current.RegisterTypeIfMissing(typeof(ToolBarView), typeof(ToolBarView), true);
            ContainerService.Current.RegisterInstance<ToolBarView>(view);

            //create headerView
            HeaderView headerView = ContainerService.Current.CreateInstance<HeaderView>();
            ContainerService.Current.RegisterTypeIfMissing(typeof(HeaderView), typeof(HeaderView), true);
            ContainerService.Current.RegisterInstance<HeaderView>(headerView);


            MessageView messageView = ContainerService.Current.CreateInstance<MessageView>();
            ContainerService.Current.RegisterTypeIfMissing(typeof(MessageView), typeof(MessageView), true);
            ContainerService.Current.RegisterInstance<MessageView>(messageView);
        }

        protected override DependencyObject CreateShell()
        {
            ActivityGlobalConfig.IsEditMode = false;
            UIDispatcher.Dispatcher = Dispatcher.CurrentDispatcher;
            return  Container.Resolve<MainWindow>();
        }
    }
}

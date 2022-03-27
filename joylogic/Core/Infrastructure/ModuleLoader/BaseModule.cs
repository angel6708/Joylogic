using Core.Infrastructure.Services;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.ViewModels;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Events;
using Unity;

namespace Core.Infrastructure.ModuleLoader
{
    public abstract class BaseModule : IModule, IModuleLifeServiceSupport
    {
        protected IRegionManager _regionManager;
        protected IEventAggregator _eventAggregator;
        protected IUnityContainer _mainContainer;

        public BaseModule(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer mainContainer)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _mainContainer = mainContainer;
        }

        public void Initialize()
        {
            BefforInitialize();
            var evt = _eventAggregator.GetEvent<MainRegionNavigateEvent>();
            evt.Subscribe(ChangeMainView, ThreadOption.UIThread, true);

            AfterInitialize();
        }

        protected abstract System.Reflection.Assembly ExecAssembly { get; }


        protected virtual void BefforInitialize() { }
        protected virtual void AfterInitialize() { }

        protected virtual bool IsPublicModule { get { return false; } }

        private Dictionary<string, Type> viewDics = null;
        private IDictionary<string, Type> GetViewNameToViewTypeMap()
        {
            if (viewDics == null)
            {
                viewDics = new Dictionary<string, Type>();
                var assembly = ExecAssembly;
                var regableType = typeof(IBaseView);
                foreach (var t in assembly.DefinedTypes)
                {
                    if (t.IsClass && t != typeof(BaseView) && regableType.IsAssignableFrom(t))
                    {
                        viewDics.Add(t.Name, t);
                    }
                }
            }
            return viewDics;
        }

        private void ChangeMainView(OperationEventArgs<MainRegionNavigateEventArgs> args)
        {

            if (args.TargetModel != null && !args.Handled)
            {
                var viewNameToViewTypeMap = GetViewNameToViewTypeMap();
                if (viewNameToViewTypeMap == null) { return; }

                if (viewNameToViewTypeMap.ContainsKey(args.TargetModel.Intent.ViewName))
                {
                    var region = _regionManager.Regions[RegionNames.MainRegion];
                    var lastView = region.ActiveViews.FirstOrDefault();

                    var views = _mainContainer.ResolveAll(viewNameToViewTypeMap[args.TargetModel.Intent.ViewName]);
                    var view = views.FirstOrDefault();
                    if (view == null)
                    {
                        view = ContainerService.Current.GetInstance(viewNameToViewTypeMap[args.TargetModel.Intent.ViewName]);
                        ContainerService.Current.RegisterTypeIfMissing(viewNameToViewTypeMap[args.TargetModel.Intent.ViewName], viewNameToViewTypeMap[args.TargetModel.Intent.ViewName], true);
                        ContainerService.Current.RegisterInstance(viewNameToViewTypeMap[args.TargetModel.Intent.ViewName], view);
                    }

                    if (lastView != null)
                    {
                        region.Remove(lastView);
                    }
                    var navigateItem = view as INavigationPanelItem;
                    if (navigateItem != null)
                    {
                        var intent = args.TargetModel.Intent;
                        var navBehavior = args.TargetModel.Intent.NavigationBehavior;

                        if ((intent.NavigationBehavior & NavigationBehavior.Animation) == NavigationBehavior.Animation)
                        {
                            if (intent.IsFromCancel) { navigateItem.NavigationBehavior = NavigationBehavior.Previous; }
                            else { navigateItem.NavigationBehavior = NavigationBehavior.Next; }
                        }
                        else
                        {
                            navigateItem.NavigationBehavior = NavigationBehavior.Replace;
                        }
                    }

                    var baseView = view as BaseView;
                    if (baseView != null)
                    {
                        var viewModel = baseView.DataContext as BaseViewModel;
                        if (viewModel != null)
                        {
                            viewModel.Intent = args.TargetModel.Intent;

                            //如果添加和卸载的页面相同，刷新ViewName
                            if (baseView == lastView)
                            {
                                if (args.TargetModel.Intent != null)
                                {
                                    var name = viewModel.Intent.ViewShownName;
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        viewModel.RefreshHeaderName(name);
                                    }
                                }
                            }
                        }
                    }



                    region.Add(view, args.TargetModel.Intent.ViewName);
                    region.Activate(view);

                    args.Handled = true;

                    if (!IsPublicModule)
                    {
                        var moduleLifeService = ContainerService.Current.GetInstance<ModuleLifeService>();
                        if (moduleLifeService.CurrentModule != this)
                        {
                            var last = moduleLifeService.CurrentModule;
                            if (last != null)
                            {
                                last.OnDisactive();
                            }
                            moduleLifeService.CurrentModule = this;
                            moduleLifeService.CurrentModule.OnActive();
                        }
                    }

                }
            }

        }

        public virtual void OnActive()
        {
        }

        public virtual void OnDisactive()
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.Initialize();
        }
    }
}

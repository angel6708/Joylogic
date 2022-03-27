
using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.ModuleLoader;
using Core.Infrastructure.Services;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using TestModule.WorkFlow;
using Unity;

namespace TestModule
{
    public class TestModule : BaseModule
    {
        ISysNavigationService _navigationService;
        public TestModule(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer mainContainer, ISysNavigationService navigationService)
            : base(regionManager, eventAggregator, mainContainer)
        {
            this._navigationService = navigationService;

        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize();
            //Register types here , view types are auto registered 
            //you can register type for lazy create
            ContainerService.Current.RegisterInstance<ITestCaseSession>(new TestCaseSession());
            ContainerService.Current.RegisterTypeIfMissing<ITestCaseSession4View, TestCaseSession4View>(true );
            ContainerService.Current.RegisterTypeIfMissing<ITestCondition, TestCondition>(true);
            ContainerService.Current.RegisterTypeIfMissing<TestContext>(true);
            ContainerService.Current.RegisterTypeIfMissing<ITestForeachSession, TestForeachSession>(true);
            ContainerService.Current.RegisterTypeIfMissing<ITestSaveServie, TestSaveService>(true);
            ContainerService.Current.RegisterTypeIfMissing<ITestSaveServieFalse, TestSaveServiceFalse>(true);

        }


        protected override System.Reflection.Assembly ExecAssembly
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly(); }
        }
    }
}

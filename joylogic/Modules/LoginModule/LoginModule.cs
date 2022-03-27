
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
using Unity;

namespace LoginModule
{
    public class LoginModule : BaseModule
    {
        ISysNavigationService _navigationService;
        public LoginModule(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer mainContainer, ISysNavigationService navigationService)
            : base(regionManager, eventAggregator, mainContainer)
        {
            this._navigationService = navigationService;

        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize(); 
             
        }
         

        protected override System.Reflection.Assembly ExecAssembly
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly(); }
        }
    }
}

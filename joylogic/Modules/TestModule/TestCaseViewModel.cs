using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using System.Threading;
using System.Windows.Threading;
using Core.Infrastructure.ViewModels;

using Core.Infrastructure.Workflow;
using Data.Models;
using Data.Access.BLL;
using Data.Access.Interface;
using System.Transactions;
using Prism.Regions;
using Prism.Events;
using Core.Infrastructure.Logging;

namespace TestModule
{
    public class TestCaseViewModel : BaseCaseViewModel<Views ,ITestCaseSession4View>
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private ISysNavigationService _sysNavigationService;
        private Dispatcher _uiDispatcher;
        private ILoggerFacade _logger;
        private CurrentContextSessionService _userSessionService;
   

        public TestCaseViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ISysNavigationService sysNavigationService, CurrentContextSessionService userSessionService, ILoggerFacade logger)
        {
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _sysNavigationService = sysNavigationService;
            _userSessionService = userSessionService;
            _logger = logger; 
        }

        protected override IReadOnlyCollection<Core.Infrastructure.Controls.IconTextDelegateCommand<string>> GetToolBarCommands()
        {
            var cmds= new List<Core.Infrastructure.Controls.IconTextDelegateCommand<string>>();
            cmds.Add(new Core.Infrastructure.Controls.IconTextDelegateCommand<string>("Foo", s => 
            {
                this.GotoCase( Views.Foo );
            }));

            cmds.Add(new Core.Infrastructure.Controls.IconTextDelegateCommand<string>("Bar", s =>
            {
                this.GotoCase( Views.Bar);
            }));

             
            cmds.Add(new Core.Infrastructure.Controls.IconTextDelegateCommand<string>("Back", s =>
            {
                this.Intent.Cancel();
            }));
            return cmds;
        }
 

        protected override void Actived()
        { 
            base.Actived(); 
             
        }

        protected override void Disactived()
        {
            base.Disactived();

        }

        protected override bool IsShowHeader
        {
            get
            {
                return false;
            }
        }

        protected override bool IsSearchEnabled
        {
            get
            {
                return false;
            }
        }

        protected override string GetInitialHeaderName()
        {
            return "TestView";
        }
  
    }
}

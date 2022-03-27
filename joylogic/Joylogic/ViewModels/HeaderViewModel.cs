using Core.Infrastructure.Consts;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows;
using Core.Infrastructure.ViewModels;
using Core.Infrastructure;
using Prism.Mvvm;
using Prism.Events;

namespace Joylogic.ViewModels
{
    public class HeaderViewModel : BindableBase, IViewModel
    {
        private ISysNavigationService _navigationService = null;
        private CurrentContextSessionService _userSessionService;
        IEventAggregator _eventAggregator;
         

        public HeaderViewModel(ISysNavigationService navigationService, IEventAggregator eventAggregator, CurrentContextSessionService userSessionService) :
            base()
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _eventAggregator = eventAggregator; 
             
        }

   
        public int HeaderSearchMaxLength { get { return 200; } }

    }
}

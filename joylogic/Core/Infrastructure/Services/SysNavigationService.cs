
using Core.Infrastructure.Services;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.Workflow;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Events;
using Unity;

namespace Core.Infrastructure
{

    public interface ISysNavigationService
    {
        void ChangeMainRegion(Intent intent);
        void ChangeHeaderRegion(string headerName, bool isVisble, bool isShowSearch = true,string searchHintText=null);
        IReadOnlyCollection<IconTextDelegateCommand<string>> GetToolBarCommands();
        void SetToolBarCommands(IEnumerable<IconTextDelegateCommand<string>> commands);

    }

    public class SysNavigationService : ISysNavigationService
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IUnityContainer _mainContainer;

        public SysNavigationService(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer mainContainer)
        {
            _mainContainer = mainContainer;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void ChangeMainRegion(Intent intent)
        {
            var evt = _eventAggregator.GetEvent<MainRegionNavigateEvent>();
            evt.Publish(new OperationEventArgs<MainRegionNavigateEventArgs>
            {
                TargetModel = new MainRegionNavigateEventArgs()
                {
                    Intent = intent
                }
            });

        }


        public void ChangeHeaderRegion(string headerName, bool isVisble, bool isShowSearch = true,string searchHintText=null)
        {
            var evt = _eventAggregator.GetEvent<ShowHeaderRegionEvent>();
            evt.Publish(new OperationEventArgs<ShowHeaderRegionEvenArgs>
            {
                TargetModel = new ShowHeaderRegionEvenArgs
                    {
                        HeaderText = headerName,
                        IsShowSearch = isShowSearch,
                        IsVisible = isVisble,
                         SearchHintText=searchHintText
                    }
            });
        }



        public IReadOnlyCollection<IconTextDelegateCommand<string>> GetToolBarCommands()
        {
            return _toolBarCommands;
        }

        private ObservableCollection<IconTextDelegateCommand<string>> _toolBarCommands;

        public void SetToolBarCommands(IEnumerable<IconTextDelegateCommand<string>> commands)
        {
            if (commands == null)
            {
                _toolBarCommands = new ObservableCollection<IconTextDelegateCommand<string>>();
            }
            else
            {
                _toolBarCommands = new ObservableCollection<IconTextDelegateCommand<string>>(commands);
            }


            var evt = _eventAggregator.GetEvent<ToolBarCommandChangedEvent>();
            evt.Publish(new OperationEventArgs<IReadOnlyCollection<IconTextDelegateCommand<string>>> { TargetModel = _toolBarCommands });


        }

   
    }
}

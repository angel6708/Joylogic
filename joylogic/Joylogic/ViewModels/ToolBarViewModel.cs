using Joylogic.Views;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using Core.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;

namespace Joylogic.ViewModels
{
    public class ToolBarViewModel : BindableBase, IViewModel
    {
        IRegionManager _regionManager;
        public ToolBarViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) :
            base()
        {
            _regionManager = regionManager;
            var evt = eventAggregator.GetEvent<ToolBarCommandChangedEvent>();
            evt.Subscribe(OnToolBarCommandsChanged, true);
        }


        private void OnToolBarCommandsChanged(bool hasItem)
        {
            lock (this)
            {
                var region = _regionManager.Regions[RegionNames.MainToolBarRegion];
                var lastView = region.ActiveViews.FirstOrDefault();
                if (lastView != null) { region.Remove(lastView); }
                var view = ContainerService.Current.GetInstance<ToolBarView>();
                
                if (hasItem)
                {
                    region.Add(view);
                    region.Activate(view);
                }
            }
        }

        private void OnToolBarCommandsChanged(OperationEventArgs<IReadOnlyCollection<IconTextDelegateCommand<string>>> obj)
        {
            OnToolBarCommandsChanged(obj.TargetModel.Count != 0);

            this._toolBarCommands = obj.TargetModel.ToList(); 
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs (nameof(ToolBarCommands)));

            if (this._toolBarCommands != null)
            {
                foreach (var cmd in this.ToolBarCommands)
                {
                    if (cmd is IconTextDelegateCommand<string>)
                    {
                        var iconCmd = cmd as IconTextDelegateCommand<string>;
                        if (iconCmd != null && iconCmd.IsSelectedDifferent && iconCmd.IsActive)
                        {
                            iconCmd.HasSelected = true;
                            break;
                        }
                    }
                }
            }

        }

        private IReadOnlyCollection<ICommand> _toolBarCommands;
        public IReadOnlyCollection<ICommand> ToolBarCommands
        {
            get
            {
                return _toolBarCommands;

            }
        }
        
    }
}

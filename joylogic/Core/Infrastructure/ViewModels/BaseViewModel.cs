
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using Core.Infrastructure.Workflow;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Core.Infrastructure.ViewModels
{
    public interface IViewModel
    {

    }

    public abstract class BaseViewModel : BindableBase, IViewModel, IDisposable
    {
        #region AciveProperty

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                bool lastState = _isActive;

                _isActive = value;
                if (lastState != value)
                {
                    if (_isActive)
                    {
                        OnActived();
                    }
                    else
                    {
                        OnDisactived();
                    }
                }
                this.RaisePropertyChanged(() => this.IsActive);
            }
        }

        protected void RaisePropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
        {
            base.RaisePropertyChanged(propertyExpression.Name);
        }

        private void OnPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression) 
        {
            base.RaisePropertyChanged(propertyExpression.Name);
        }

        public Intent Intent { get; set; }

        public event Action<bool> ActiveChanged;
        protected void OnActiveChanged()
        {
            if (ActiveChanged != null)
            {
                ActiveChanged(this.IsActive);
            }
        }

        #endregion


        private void OnActived()
        {
            var eventAggregator = ContainerService.Current.GetInstance<IEventAggregator>();
            var searchEvent = eventAggregator.GetEvent<SearchEvent>();
            searchEvent.Subscribe(new Action<OperationEventArgs<string>>(OnSearch));
            var navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            navigationService.ChangeHeaderRegion(this.HeaderName, this.IsShowHeader, this.IsSearchEnabled, this.SearchHintText);

            Actived();
            RefreshToolBarCommands();
            OnActiveChanged();
        }
        private void OnDisactived()
        {
            var eventAggregator = ContainerService.Current.GetInstance<IEventAggregator>();
            var searchEvent = eventAggregator.GetEvent<SearchEvent>();
            searchEvent.Unsubscribe(new Action<OperationEventArgs<string>>(OnSearch));
            Disactived();
            OnActiveChanged();
        }


        protected void RefreshToolBarCommands(string selectText = "")
        {
            var cmds = GetToolBarCommands();
            if (!string.IsNullOrEmpty(selectText))
            {
                var selected = cmds.Where(a => a.Text == selectText).FirstOrDefault();

                if (selected != null)
                {
                    selected.IsActive = true;
                }
            }
            var navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            navigationService.SetToolBarCommands(cmds);
        }

        private void OnSearch(OperationEventArgs<string> arg)
        {
            if (IsSearchEnabled && this.IsActive)
            {
                DoSerch(arg.TargetModel);
            }
        }

        protected virtual IReadOnlyCollection<IconTextDelegateCommand<string>> GetToolBarCommands()
        {
            var navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            return navigationService.GetToolBarCommands();
        }

        protected virtual bool IsSearchEnabled { get { return false; } }

        protected virtual bool IsShowHeader { get { return true; } }

        private string _headerName = string.Empty;

        private bool _headNameInited = false;
        public string HeaderName
        {
            get
            {
                if (!_headNameInited) { _headerName = this.GetInitialHeaderName(); }
                return _headerName;
            }
        }

        protected virtual string SearchHintText { get { return "请输入..."; } }

        protected virtual string GetInitialHeaderName() { return string.Empty; }
        public void RefreshHeaderName(string newName)
        {
            _headerName = newName;
            var navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            navigationService.ChangeHeaderRegion(newName, this.IsShowHeader, this.IsSearchEnabled, this.SearchHintText);
        }

        /// <summary>
        /// this Method Only Exec once if IsActive  Prop is not changed,even if you do set IsActive property more than once
        /// </summary>
        protected virtual void Actived()
        {
           
        }

        protected void Refresh()
        {
            this.OnDisactived();
            this.OnActived();
        }


        /// <summary>
        /// this Method Only Exec once if IsActive  Prop is not changed,even if you do set IsActive property more than once
        /// </summary>
        protected virtual void Disactived()
        {

        }


        protected virtual void DoSerch(string filter)
        {

        }

        public virtual void Dispose()
        {

        }


        public void ShowMessage(string dialogCaption, Action confrimCommandHandler=null, Action cancelCommandHandler = null,TimeSpan during= default(TimeSpan), MessageType messageType= MessageType.Info)
        {
            var eventAggregator = ContainerService.Current.GetInstance<IEventAggregator>();
            var showMessageEvent = eventAggregator.GetEvent<ShowMessageEvent>();
            showMessageEvent.Publish(
                new OperationEventArgs<ShowMessageEventArg>
                {
                    TargetModel = new ShowMessageEventArg()
                    {
                         MessageType= messageType,
                         During= during,
                        Message = dialogCaption,
                        OkCommandHandler = confrimCommandHandler,
                        CancelCommandHandler = cancelCommandHandler
                    }
                });
        }


        public void BeginLoading(string caption)
        {
            IEventAggregator _eventAggregator = ContainerService.Current.CreateInstance<IEventAggregator>();
            _eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
            {
                Status = Visibility.Visible,
                Caption = caption,
            });
        }

        public void EndLoading()
        {
            IEventAggregator _eventAggregator = ContainerService.Current.CreateInstance<IEventAggregator>();
            _eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
            {
                Status = Visibility.Collapsed,
            });
        }


    }
}

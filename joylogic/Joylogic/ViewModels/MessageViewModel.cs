using Joylogic.Views;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using Core.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using Prism.Commands;

namespace Joylogic.ViewModels
{
    public class MessageViewModel : BindableBase, IViewModel
    {

        private Timer _searchTimer;
        private IRegionManager _regionManager;

        private Dispatcher _uiDispatcher;
        private Action OkCommandHandler { get; set; }
        private Action CancelCommandHandler { get; set; }


        public MessageViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _regionManager = regionManager;
            var evt = eventAggregator.GetEvent<ShowMessageEvent>();
            evt.Subscribe(ShowMessageChanged, ThreadOption.UIThread, true);
            _searchTimer = new Timer(new TimerCallback(CloseMessageView));

            
            CloseClick = new DelegateCommand<object>(CloseMessageView);

            OKCommand = new DelegateCommand<object>(OKCloseMessageView);
            CancelCommand = new DelegateCommand<object>(CannelCloseMessageView);


        }

        private void OKCloseMessageView(object state)
        {

            CloseMessageView(state);

            if (OkCommandHandler != null) { var cb = OkCommandHandler; OkCommandHandler = null; cb(); }

        }
        private void CannelCloseMessageView(object state)
        {
            CloseMessageView(state);
            if (CancelCommandHandler != null) {  var cb = CancelCommandHandler; CancelCommandHandler = null; cb(); }
        }


        private void CloseMessageView(object state)
        {
            var region = _regionManager.Regions[RegionNames.MessageRegion];
            var lastView = region.ActiveViews.FirstOrDefault();
            if (lastView != null)
            {
                _uiDispatcher.Invoke((Action)delegate() {

                    region.Remove(lastView);
                });
            }
        }

        private Brush messageColor;
        public Brush MessageColor { 
            get { return messageColor; }
            set { messageColor = value; 
                OnPropertyChanged("MessageColor");
            } }

        private void OnPropertyChanged(string v)
        {
              this.RaisePropertyChanged(v);
        }

        private void ShowMessageChanged(OperationEventArgs<ShowMessageEventArg> obj)
        {
            lock (this)
            {
                this.OkCommandHandler = obj.TargetModel.OkCommandHandler;
                this.CancelCommandHandler = obj.TargetModel.CancelCommandHandler;
                 
                if (obj.TargetModel.During.TotalMilliseconds!=0.0)
                {
                    _searchTimer.Change((int)obj.TargetModel.During.TotalMilliseconds, System.Threading.Timeout.Infinite);
                }
                switch (obj.TargetModel.MessageType) 
                {
                    case MessageType.Error:
                        MessageColor = Brushes.DarkRed;
                        break;
                    case MessageType.Info:
                        MessageColor = Brushes.BlueViolet;
                        break;
                    case MessageType.Warnning:
                        MessageColor = Brushes.DarkOrange;
                        break;
                } 

                Message = obj.TargetModel.Message;
                var region = _regionManager.Regions[RegionNames.MessageRegion];
                var lastView = region.ActiveViews.FirstOrDefault();
              
                if (lastView == null)
                {
                    var view = ContainerService.Current.GetInstance<MessageView>();
                    _uiDispatcher.Invoke((Action)delegate()
                    {
                        region.Add(view);
                        region.Activate(view);
                    });

                   
                }

            }
        }
        
        /// <summary>
        /// 消息内容
        /// </summary>
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                this.OnPropertyChanged(nameof(Message));
            }
        }
        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        private ICommand _closeClick;
        public ICommand CloseClick
        {
            get
            {
                return _closeClick;
            }
            set
            {
                _closeClick = value;
                this.OnPropertyChanged(nameof(CloseClick));
            }
        }

        
             private ICommand _okCommand;
             public ICommand OKCommand {
                 get
                 {
                     return _okCommand;
                 }
                 set
                 {
                     _okCommand = value;
                     this.OnPropertyChanged(nameof(OKCommand));
                 }
             }
             private ICommand _cancelCommand;
             public ICommand CancelCommand
             {
                 get
                 {
                     return _cancelCommand;
                 }
                 set
                 {
                     _cancelCommand = value;
                     this.OnPropertyChanged(nameof(CancelCommand));
                 }
             }


    }
}

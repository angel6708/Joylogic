using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using Prism.Events;

namespace Joylogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private IEventAggregator _eventAggregator;

        private Dispatcher _uiDispatcher;


        private ShowLoadingEventArg _LoadingEventArg;

        public ShowLoadingEventArg LoadingEventArg
        {
            get { return _LoadingEventArg; }
            set
            {
                _LoadingEventArg = value;
                this.OnPropertyChanged("LoadingEventArg");
            }
        }

        public MainWindow(IEventAggregator eventAggregator)
            : base()
        { 

            KillProcess();
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _eventAggregator = eventAggregator;

            var showMessageEvent = eventAggregator.GetEvent<ShowMessageEvent>();
           // showMessageEvent.Subscribe(SetMessage);

            var loadingEvent = eventAggregator.GetEvent<ShowLoadingEvent>();
            loadingEvent.Subscribe(this.ShowLoading);
           
            InitializeComponent();
            this.DataContext = this;
            this.LoadingEventArg = new ShowLoadingEventArg()
            {
                Status = Visibility.Collapsed,
                Caption = "",
            };

            //keyboard init here
            Core.Infrastructure.Controls.KeyboardControl.CreateSingle(this.KeyboardArea);
            Core.Infrastructure.Controls.NumberInputControl.CreateSingle(this.KeyboardArea);
            Core.Infrastructure.Controls.DateInputControl.CreateSingle(this.KeyboardArea);

            HideMouse();

            this.Closed += MainWindow_Closed;



        }

        private void HideMouse()
        {
            //    Mouse.OverrideCursor = Cursors.None; 
        }

        private void KillProcess()
        {
            try
            {
                System.Diagnostics.Process curentProcess = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process[] pList = System.Diagnostics.Process.GetProcessesByName(curentProcess.ProcessName);

                if (pList != null && pList.Count() > 0)
                {
                    foreach (var p in pList)
                    {
                        if (curentProcess.Id == p.Id)
                        {
                            continue;
                        }
                        p.Kill();
                    }
                }
            }
            catch
            {

            }
        }
        void MainWindow_Closed(object sender, EventArgs e)
        {
            var evt = this._eventAggregator.GetEvent<DisposeEvent>();
            evt.Publish(true);
        }

        private Action _confrimCommandHandler;
        private Action _cancelCommandHandler;

        private void SetMessage(ShowMessageEventArg messageArg)
        {
            //this._confrimCommandHandler = messageArg.OkCommandHandler;
            //this._cancelCommandHandler = messageArg.CancelCommandHandler;
            //_uiDispatcher.Invoke((Action)delegate()
            //{
            //    //messageText.Text = messageArg.Message;
            //    //messagePanel.Visibility = System.Windows.Visibility.Visible;
            //    //cancelBtn.Visibility = _cancelCommandHandler == null ? Visibility.Collapsed : Visibility.Visible;
            //});

        }

        private void confrimBtn_Click(object sender, RoutedEventArgs e)
        {

           // messageText.Text = string.Empty;
            messagePanel.Visibility = System.Windows.Visibility.Collapsed;
            if (_confrimCommandHandler != null)
            {
                _confrimCommandHandler();
                _confrimCommandHandler = null;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
           // messageText.Text = string.Empty;
            messagePanel.Visibility = System.Windows.Visibility.Collapsed;
            if (_cancelCommandHandler != null)
            {
                _cancelCommandHandler();
                _cancelCommandHandler = null;
            }

        }


        private void ShowLoading(ShowLoadingEventArg e)
        {
            this.LoadingEventArg = e;
        }



        #region event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
}

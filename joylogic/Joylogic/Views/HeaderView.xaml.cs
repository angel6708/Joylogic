using Joylogic.ViewModels;
using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.ViewModels;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Infrastructure.Utils;
using Prism.Regions;
using Prism.Events;

namespace Joylogic.Views
{
    /// <summary>
    /// Interaction logic for HeaderView.xaml
    /// </summary>
    public partial class HeaderView : IBaseView
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private Timer _searchTimer;
        private int KEY_PRESS_FREZE_INTERVAL = 500;
        private DateTime _lastPressed = DateTime.Now;


        public HeaderViewModel ViewModel { get; set; }
        public HeaderView(HeaderViewModel vm, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.DataContext = vm;
            this.ViewModel = vm;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            var evt = eventAggregator.GetEvent<ShowHeaderRegionEvent>();
            evt.Subscribe(LoadMe, ThreadOption.UIThread, true);
            InitializeComponent();

            this.searchBox.TextChanged += searchBox_TextChanged;
            _searchTimer = new Timer(new TimerCallback(_searchTimer_Elapsed));
        }
        void _searchTimer_Elapsed(object stare)
        {
            UIDispatcher.Dispatcher.Invoke((Action)delegate()
            {
                _eventAggregator.GetEvent<SearchEvent>().Publish(new OperationEventArgs<string>() { TargetModel = this.searchBox.Text });
            });
        }
        void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _lastPressed = DateTime.Now;
            _searchTimer.Change(KEY_PRESS_FREZE_INTERVAL, System.Threading.Timeout.Infinite);

        }

        private void LoadMe(OperationEventArgs<ShowHeaderRegionEvenArgs> obj)
        {
            if (obj == null) return;
            if (obj.TargetModel == null) return;

            var region = _regionManager.Regions[RegionNames.MainHeaderRegion];
            if (obj.TargetModel.IsVisible)
            {
                this.searchBox.Visibility = obj.TargetModel.IsShowSearch ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                this.searchBox.Hint = obj.TargetModel.SearchHintText;
                this.headerText.Text = obj.TargetModel.HeaderText;
                var view = region.GetView(nameof(HeaderView));
                if (view == null)
                {
                    view = this;
                    region.Add(this, nameof(HeaderView));
                }
                region.Activate(view);
            }
            else
            {
                var view = region.GetView(nameof(HeaderView));
                if (view != null)
                {
                    region.Remove(view);
                }
            }
        }

        

    }
}

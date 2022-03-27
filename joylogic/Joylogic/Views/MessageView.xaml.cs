using Joylogic.ViewModels;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.Regions;
using Prism.Events;

namespace Joylogic.Views
{
    /// <summary>
    /// MessageView.xaml 的交互逻辑
    /// </summary>
    public partial class MessageView : IBaseView
    {
        private IRegionManager _regionManager;
        public MessageViewModel ViewModel { get; set; }
        public MessageView(MessageViewModel vm, IEventAggregator eventAggregator, IRegionManager regionManager)
            : base()
        {
            _regionManager = regionManager;

            this.DataContext = vm;
            this.ViewModel = vm;
         
            InitializeComponent(); 
           
        }

    }
}

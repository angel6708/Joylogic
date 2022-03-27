
using Core.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Core.Infrastructure.Controls
{
    public abstract class BaseView : UserControl, INavigationPanelItem,IBaseView
    {
        public BaseView()
        {
            this.Loaded += BaseView_Loaded;
            this.Unloaded += BaseView_Unloaded;
 
        }



        void BaseView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseViewModel model = this.ViewModel as BaseViewModel;
            if (model != null)
            {
                model.IsActive = false;
            }
        }

        void BaseView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseViewModel model = this.ViewModel as BaseViewModel;

            if (model != null)
            {
                model.IsActive = true;

            }
        }





        public IViewModel ViewModel
        {
            set { this.DataContext = value; }
            private get { return this.DataContext as IViewModel; }
        }

        private NavigationBehavior _navigationBehavior = NavigationBehavior.Replace;
        public NavigationBehavior NavigationBehavior
        {
            get { return _navigationBehavior; }
            set { _navigationBehavior = value; }
        }

 
    }
}

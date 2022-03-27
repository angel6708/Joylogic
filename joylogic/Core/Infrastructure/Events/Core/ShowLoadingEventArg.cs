using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Core.Infrastructure.Events
{
    public class ShowLoadingEventArg : BindableBase
    {
        private string _Caption;
        public string Caption
        {
            get { return _Caption; }
            set
            {
                _Caption = value;
                this.RaisePropertyChanged(nameof(Caption));
            }
        }

        private Visibility _Status;
        public Visibility Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                this.RaisePropertyChanged(nameof(Status));
            }
        }


        private SolidColorBrush _Foreground;
        public SolidColorBrush Foreground
        {
            get { return _Foreground; }
            set
            {
                _Foreground=value;
                this.RaisePropertyChanged(nameof(Foreground));
            }
        }


        public ShowLoadingEventArg()
        {
            Foreground = Brushes.Gray;
        }


        public static SolidColorBrush WarningBrush = Brushes.Blue;

        public static SolidColorBrush ErrorBrush = Brushes.Red;

    }


}

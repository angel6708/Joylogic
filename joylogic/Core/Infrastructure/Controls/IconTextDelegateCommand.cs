
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core.Infrastructure.Controls
{


    public class IconTextDelegateCommand<T> : DelegateCommand<T>, INotifyPropertyChanged
    {


        INotifyPropertyChanged execTargetHolder = null;


        public IconTextDelegateCommand(string unSelectedBitmap, string selectedBitmap, string text, Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isSelectedDifferent, bool subMenuFlag)
            : base(executeMethod, canExecuteMethod ?? (obj => true))
        {
            SubMenuFlag = subMenuFlag;
            if (!string.IsNullOrEmpty(unSelectedBitmap))
                this.UnSelectedBitmap = new Uri(unSelectedBitmap);
            if (!string.IsNullOrEmpty(selectedBitmap))
                this.SelectedBitmap = new Uri(selectedBitmap);

            this.IsSelectedDifferent = isSelectedDifferent;

            this._text = text;
            execTargetHolder = executeMethod.Target as INotifyPropertyChanged;
            if (execTargetHolder != null)
            {
                execTargetHolder.PropertyChanged += execTargetHolder_PropertyChanged;
            }
        }

        public IconTextDelegateCommand(string text, Action<T> executeMethod, bool isSelectedDifferent = false)
            : this(null, null, text, executeMethod, null, isSelectedDifferent, false)
        {

        }

        public IconTextDelegateCommand(string text, Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isSelectedDifferent = false)
            : this(null, null, text, executeMethod, canExecuteMethod, isSelectedDifferent, false)
        {

        }



        void execTargetHolder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }


        private bool _isSelectedDifferent;
        public bool IsSelectedDifferent
        {
            get { return _isSelectedDifferent; }
            set
            {
                _isSelectedDifferent = value;

            }
        }

        private bool _HasSelected;
        public bool HasSelected
        {
            get { return _HasSelected; }
            set
            {
                _HasSelected = value;
                this.OnPrepertyChanged("HasSelected");
            }
        }


        private string _text;
        public string Text
        {
            get { return _text; }
            set 
            {
                _text = value;
                this.OnPrepertyChanged("Text");
            }

        }

        public bool SubMenuFlag { get; private set; }

        public Uri UnSelectedBitmap { get; private set; }
        public Uri SelectedBitmap { get; private set; }

        protected void OnPrepertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

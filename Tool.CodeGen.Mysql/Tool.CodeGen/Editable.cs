using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class Editable : INotifyPropertyChanged
    {

        private bool _selected=true;
        public bool Selected
        {
            get { return _selected; }

            set
            {
                _selected = value;

                if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Selected")); }
            }
        }
        private string _aName;
        public string AName
        {
            get { return _aName; }
            set
            {
                _aName = value;
                OnPropertyChanged("AName");
            }
        }

        protected void OnPropertyChanged(string p) 
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

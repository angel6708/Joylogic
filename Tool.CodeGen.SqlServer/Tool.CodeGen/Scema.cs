using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class Schema : INotifyPropertyChanged
    {
        public string Name { get; set; }
         

        private List<Ref> _cols;
        public List<Ref> Refs
        {
            get { return _cols; }
            set
            {
                _cols = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Refs"));
                }
            }
        }


        private List<Table> _cols1;
        public List<Table> Tables
        {
            get { return _cols1; }
            set
            {
                _cols1 = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tables"));
                }
            }
        }


        private List<Table> _cols12;
        public List<Table> Views
        {
            get { return _cols12; }
            set
            {
                _cols12 = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Views"));
                }
            }
        }


        public override string ToString()
        {
             return  Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

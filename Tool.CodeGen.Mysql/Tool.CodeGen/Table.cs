using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class Table : Editable
    {

        //public List<string> Enums { get; set; }

        public Table()
        {
            this.Selected = true;
        }


        public string SnapshotTableName { get; set; }
        public string SnaoshotTableBaseName { get; set; }
        public bool IsView { get; set; }
        public bool IsSnapshot { get; set; }
        public bool isSnapshotBase { get; set; }

        public string Name { get; set; }

        private List<Column> _cols;
        public List<Column> Columns
        {
            get { return _cols; }
            set
            {
                _cols = value;
                OnPropertyChanged("Columns");
            }
        }

        public List<Ref> Refs { get; set; }


        public override string ToString()
        {
            return Name;
        }

    }
}

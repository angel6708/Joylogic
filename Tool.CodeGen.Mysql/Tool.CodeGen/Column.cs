using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class Column : Editable
    {

        public string Desc { get; set; }
        public string Name { get; set; }

        public string DbType { get; set; }
        private string _lanType = null;
        public string LanType
        {
            get
            {
                if (_lanType == null)
                {
                    _lanType = TypeRef.FromDbType(DbType,ColLen);
                }
                return _lanType;
            }
            set
            {
                _lanType = value;
            }
        }

        private bool _isPk;
        public bool IsPK
        {
            get { return _isPk; }
            set
            {
                _isPk = value;
                OnPropertyChanged("IsPK");
            }
        }

        public int ColLen { get; set; }

        public bool IsNullable { get; set; }

    }
}

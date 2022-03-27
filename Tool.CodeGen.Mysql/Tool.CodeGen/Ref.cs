using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{

    public enum RefType 
    {
    
        O2M,
        O2O,
        M2M
    }

    public class Ref : Editable
    {
        public string OneTable { get; set; }

        public string ManyTable { get; set; }

        public string OneTabCol { get; set; }

        public string ManyTabCol { get; set; }

        public RefType RefType { get; set; }

        public Table OneTableEntity { get; set; }

    }
}

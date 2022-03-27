using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }

        public string KeyColumn { get; set; }

        public bool IsSnapshot { get; set; }
    }
}

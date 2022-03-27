using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Models
{
    public interface IModel
    {
        Guid? CreateBy { get; set; }
        Guid? UpdateBy { get; set; }

        DateTime CreateTime { get; set; }

        DateTime? UpdateTime { get; set; }

        Guid Key { get; set; }

        string Ext01 { get; set; }
        string Ext02 { get; set; }
        string Ext03 { get; set; }
        string Ext04 { get; set; }
        string Ext05 { get; set; }

        string SourceId { get; set; }
        object GetParam();

    }
}

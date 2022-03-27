using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace DAL
{
    /// <summary>
    /// 自定义的字段数据类型枚举
    /// </summary>
    public enum CustomDbType
    {
        String = 1,
        Int16 = 2,
        Int32 = 3,
        Single = 4,
        DateTime = 5,
        Guid = 6,
        Boolean = 7
    }
}

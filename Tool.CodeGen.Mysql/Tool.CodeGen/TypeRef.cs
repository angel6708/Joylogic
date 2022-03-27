using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class TypeRef
    {
        public static string FromDbType(string dbType, int len)
        {
            switch (dbType.ToUpper())
            {
                case "BFILE":
                case "BLOB":
                case "LONG RAW":
                case "RAW":
                    return "byte[]";
                case "CHAR":
                    if (len == 36) { return "Guid"; }
                    if (len == 5) {return "bool"; }
                   // if (len == 10) { return "enum"; }
                    return "string";
                case "CLOB":
                case "LONG":
                case "NCHAR":
                case "NCLOB":
                case "NVARCHAR":
                case "VARCHAR":
                case "NVARCHAR2":
                    return "string";
                case "ROWID":
                case "VARCHAR2":
                    return "string";
                case "FLOAT":
                case "INTEGER":
                case "INT":
                case "NUMBER":
                    {

                        return "Decimal";
                    }

                case "DATE":
                case "TIMESTAMP(6)":
                case "DATETIME":

                    return "DateTime";

                case "INTERVAL YEAR TO MONTH":

                    return "int";
                case "INTERVAL DAY TO SECOND":

                    return "TimeSpan";
                case "TINYINT":
                    return "bool";

                default: return "string";
            }
        }


    }
}

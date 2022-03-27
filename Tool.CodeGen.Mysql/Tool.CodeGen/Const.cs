using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class Const
    {
        public const string SCHEMA_SQL = " select scama_name as USERNAME from information_schema.SCHEMATA ";

        public const string SCHEMA_TABS = "select TABLE_NAME from information_schema.TABLES where TABLE_SCHEMA=@owner ";
        public const string SCHEMA_VWS = "select VIEW_NAME from dba_views where owner=:owner";

        public const string TAB_DESC = @"select
COLUMN_NAME,
DATA_TYPE,
ifnull(character_maximum_length,0) DATA_LENGTH,
ifnull(IS_NULLABLE,'Y') NULLABLE,
ordinal_position COLUMN_ID,
ifnull(COLUMN_KEY,'') CONSTRAINT_TYPE ,
ifnull(COLUMN_COMMENT,'') COLUMN_COMMENT
 from  information_schema.columns where TABLE_SCHEMA=@owner and table_name=@table_name";

        public const string TAB_REF = @"select DISTINCT UC2.TABLE_NAME as ONETAB,UC.TABLE_NAME AS MANYTAB ,ACC2.COLUMN_NAME AS ONECOL,UCC.COLUMN_NAME AS MANYCOL from  ALL_constraints uc inner join  ALL_constraints uc2
on UC.R_CONSTRAINT_NAME=UC2.CONSTRAINT_NAME 
INNER JOIN ALL_CONS_COLUMNS UCC ON UCC.CONSTRAINT_NAME=UC.CONSTRAINT_NAME
INNER JOIN ALL_CONS_COLUMNS ACC2 ON ACC2.CONSTRAINT_NAME=UC2.CONSTRAINT_NAME
where UC.CONSTRAINT_TYPE='R' and uc.owner=:owner";
    }

    
}

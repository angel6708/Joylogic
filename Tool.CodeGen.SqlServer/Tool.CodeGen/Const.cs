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

        public const string SCHEMA_TABS = @"select 
sys.schemas.name+'.'+sys.objects.name as TABLE_NAME
from sys.objects,
sys.schemas
where sys.objects.type='U'
and sys.objects.schema_id=sys.schemas.schema_id";

        public const string SCHEMA_VWS = "select VIEW_NAME from dba_views where owner=:owner";

        public const string TAB_DESC = @"select   
       COLUMN_NAME=t_c.name, 
        DATA_TYPE=t.name,  
         DATA_LENGTH=t_c.max_length,  
           NULLABLE=case when t_c.is_nullable=1 then 'YES' else '' end, 
          
    COLUMN_ID=t_c.column_id,  
   CONSTRAINT_TYPE=  
    (  
        case when exists  
        (  
            select 1 from sys.indexes i,sys.index_columns ic,sys.objects o  
                where o.type='PK' and o.name=i.name and i.index_id=ic.index_id   
                    and i.object_id=ic.object_id and ic.column_id=t_c.column_id   
                    and o.parent_object_id=t_c.object_id  
        )  
        then 'PRI'  
        else ''  
        end  
    ),  
    
     COLUMN_COMMENT=isnull(e.value,'') 
     
from sys.columns t_c  
    inner join sys.objects t_o on t_c.object_id=t_o.object_id          
    left join sys.types t on t.system_type_id=t_c.system_type_id   
        and t.user_type_id=t_c.user_type_id  
    left join sys.default_constraints c on c.object_id=t_c.default_object_id   
        and c.parent_object_id=t_c.object_id and c.parent_column_id=t_c.column_id  
    left join sys.extended_properties e on e.major_id=t_c.object_id   
        and e.minor_id=t_c.column_id      
    left join   
    (  
        select parent_object_id,referenced_object_id,column_id=min(key_index_id) from sys.foreign_keys  
            group by parent_object_id,referenced_object_id  
    )f on f.parent_object_id=t_c.object_id and f.column_id=t_c.column_id    
    left join sys.columns f_c on f_c.object_id=f.referenced_object_id and f_c.column_id=f.column_id  
    left join sys.objects f_o on f_o.object_id=f.referenced_object_id  
where t_o.type='U' and t_o.name<>'sysdiagrams'  
  and t_o.name =@table_name
    order by t_o.name,t_c.column_id  ";

        public const string TAB_REF = @"select DISTINCT UC2.TABLE_NAME as ONETAB,UC.TABLE_NAME AS MANYTAB ,ACC2.COLUMN_NAME AS ONECOL,UCC.COLUMN_NAME AS MANYCOL from  ALL_constraints uc inner join  ALL_constraints uc2
on UC.R_CONSTRAINT_NAME=UC2.CONSTRAINT_NAME 
INNER JOIN ALL_CONS_COLUMNS UCC ON UCC.CONSTRAINT_NAME=UC.CONSTRAINT_NAME
INNER JOIN ALL_CONS_COLUMNS ACC2 ON ACC2.CONSTRAINT_NAME=UC2.CONSTRAINT_NAME
where UC.CONSTRAINT_TYPE='R' and uc.owner=:owner";
    }

    
}

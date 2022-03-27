using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class TableInfo
    {

        private static Dictionary<Type, TableInfo> _cache;

        static TableInfo()
        {
            _cache = new Dictionary<Type, TableInfo>();
        }
        public string KeyColumn { get; internal set; }

        public string TableName { get; internal set; }

        public bool IsSnapshot { get; internal set; }

        public IReadOnlyDictionary<string, string> ColumnPropMaps { get; internal set; }

        public static TableInfo FromType(Type modelType)
        {
            if (_cache.ContainsKey(modelType)) 
            {
                return _cache[modelType];
            }

            TableInfo info = new TableInfo();
            foreach (var attr in modelType.CustomAttributes)
            {
                if (attr.AttributeType == typeof(TableAttribute))
                {
                    foreach (var item in attr.NamedArguments)
                    {
                        if (item.MemberName == "KeyColumn")
                        {
                            info.KeyColumn = item.TypedValue.Value.ToString();
                        }
                        else if (item.MemberName == "TableName")
                        {
                            info.TableName = item.TypedValue.Value.ToString();
                        }
                        else if (item.MemberName == "IsSnapshot")
                        {
                            info.IsSnapshot = (bool)item.TypedValue.Value;
                        }

                    }
                    break;
                }
            }
            Dictionary<string, string> columnMaps = new Dictionary<string, string>();


            foreach (var p in modelType.GetProperties())
            {
                foreach (var attr in p.CustomAttributes)
                {
                    if (attr.AttributeType == typeof(ColumnAttribute))
                    {
                        foreach (var item in attr.NamedArguments)
                        {
                            if (item.MemberName == "Column")
                            {
                                columnMaps.Add(item.TypedValue.Value.ToString(), p.Name);
                            }
                            continue;
                        }
                        break;
                    }
                }
            }

            info.ColumnPropMaps = columnMaps;
            _cache.Add(modelType, info);
            return info;
        }
    }
}

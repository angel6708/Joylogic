using Dapper;
using Data.Access.Interface.BLL;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Access.BLL
{
    public partial class GlobalConfigBLL : IGlobalConfigBLL
    {
        public  GlobalConfig GetGlobalConfigbyConfigKey(string ConfigKey)
        {
            var sql = this.BuildBaseSql() + " where `config_key` = @ConfigKey ";

            var GlobalConfigList =
                SqlMapper.Query<GlobalConfig>(ConnectionFactory.Current.GetSessionConnection(), sql,
                new { ConfigKey = ConfigKey });

            if (GlobalConfigList == null) 
            { return null; }

            return GlobalConfigList.FirstOrDefault();
        }
    }
}

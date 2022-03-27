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
    public partial class UserInfoBLL : IUserInfoBLL
    {

        public UserInfo Login(string loginName, string password)
        {
            var sql = this.BuildBaseSql() + " where `login_name` = @loginName and `password` =@Password";

            var userList =
                SqlMapper.Query<UserInfo>(ConnectionFactory.Current.GetSessionConnection(), sql,
                new { loginName = loginName, Password = password });

            if (userList == null) { return null; }
            return userList.FirstOrDefault();
        }

    
    }
}

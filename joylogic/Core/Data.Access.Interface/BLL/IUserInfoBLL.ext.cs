using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Data.Access.Interface;
using Data.Models;
 
namespace Data.Access.Interface.BLL
{ 
    public partial interface IUserInfoBLL:IRegable,IBaseBLL<UserInfo> 
    {
        UserInfo Login(string loginName, string password);
    }
}

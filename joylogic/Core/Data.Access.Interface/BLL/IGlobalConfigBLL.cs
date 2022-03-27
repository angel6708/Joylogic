using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Data.Access.Interface;
using Data.Models;
 
namespace Data.Access.Interface.BLL
{
    [ServiceContract(Namespace ="Data.Access.BLL")]
    public partial interface IGlobalConfigBLL:IRegable,IBaseBLL<GlobalConfig> 
    {
         GlobalConfig GetGlobalConfigbyConfigKey(string ConfigKey);
    }
}

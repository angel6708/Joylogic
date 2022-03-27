using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Data.Access;
using Data.Models;
using Data.Access.Interface.BLL; 

namespace Data.Access.BLL
{

   [ServiceBehavior( Name="GlobalConfigServiceBehavior", InstanceContextMode =InstanceContextMode.Single)]
	public partial class GlobalConfigBLL  : BaseBLL<GlobalConfig> , IGlobalConfigBLL
	{
 
    }

}
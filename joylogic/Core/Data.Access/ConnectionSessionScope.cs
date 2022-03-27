using Core.Infrastructure.Logging;
using Core.Infrastructure.Services;
using Data.Access.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Access
{
    public class ConnectionSessionScope : IConnectionSessionScope
    {
        public bool HasOpen { get; set; }
        public ConnectionSessionScope()
        {
            if (ConnectionFactory.Current.GetSessionConnection().State != System.Data.ConnectionState.Open)
            {
                try
                {
                    ConnectionFactory.Current.GetSessionConnection().Open();
                    HasOpen = true;
                }
                catch (Exception ex)
                {
                    HasOpen = false;
                    var logger = ContainerService.Current.GetInstance<ILoggerFacade>();
                    logger.Log(ex.StackTrace, Category.Exception, Priority.High);

                }
            }
        }
        public void Dispose()
        {
            ConnectionFactory.Current.CloseSessionConnection();
        }
    }
}

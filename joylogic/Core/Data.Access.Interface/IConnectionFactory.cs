using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Access.Interface
{
    public interface IConnectionFactory
    {
        IDbConnection GetSessionConnection();
        void CloseSessionConnection();
    }
}

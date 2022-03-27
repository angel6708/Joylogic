using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Data.Access.Interface
{
    public class WcfConnection : IDbConnection
    {
        private List<IChannelFactory> _channels;
        public void AppendChannel(IChannelFactory channel)
        {
            _channels.Add(channel);
        }

        public WcfConnection()
        {
            _channels = new List<IChannelFactory>();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return null;
        }

        public IDbTransaction BeginTransaction()
        {
            return null;
        }

        public void ChangeDatabase(string databaseName)
        {

        }

        public void Close()
        {
            Exception ex = null;
            foreach (var c in _channels)
            {
                try
                {
                    c.Close();
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }
            if (ex != null)
            {
                throw ex;
            }
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public int ConnectionTimeout
        {
            get;
            set;
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public string Database
        {
            get { return "WCF"; }
        }

        public void Open()
        {

        }

        public ConnectionState State
        {
            get { return ConnectionState.Open; }
        }

        public void Dispose()
        {

        }
    }
}

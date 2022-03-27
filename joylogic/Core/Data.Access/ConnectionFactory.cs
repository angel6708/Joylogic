using Data.Access.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Access
{
    public class ConnectionFactory : IConnectionFactory
    {

        public string ConnectionString { get; set; }


        private TimeSpan _timeOut = new TimeSpan(0, 0, 0, 6);
        private AutoResetEvent _maxConnCountEvt = new AutoResetEvent(true);
        private int MAX_CONN_COUNT = 1;

        /// <summary>
        /// one session per thread ;
        /// </summary>
        private IDictionary<int, IDbConnection> _sessionCache;

        private static byte[] _locker = new byte[0];

        private static ConnectionFactory _current = null;
        public static ConnectionFactory Current
        {
            get
            {
                lock (_locker)
                {
                    if (_current == null)
                    {
                        _current = new ConnectionFactory();
                    }
                    return _current;
                }
            }
        }

        private ConnectionFactory()
        {
            _sessionCache = new Dictionary<int, IDbConnection>();
        }

        public IDbConnection GetSessionConnection()
        {


            if (_sessionCache.ContainsKey(Thread.CurrentThread.ManagedThreadId))
            {
                IDbConnection st = _sessionCache[Thread.CurrentThread.ManagedThreadId];
                return st;
            }
            else
            {
                if (_sessionCache.Count >= MAX_CONN_COUNT)
                {
                    _maxConnCountEvt.WaitOne(_timeOut);
                }
                lock (this)
                {
                    if (false)
                    {
                        _sessionCache.Add(Thread.CurrentThread.ManagedThreadId, new WcfConnection());
                        return _sessionCache[Thread.CurrentThread.ManagedThreadId];
                    } 
                    else
                    {
                        _sessionCache.Add(Thread.CurrentThread.ManagedThreadId,
                           new MySql.Data.MySqlClient.MySqlConnection  (ConnectionString));
                        return _sessionCache[Thread.CurrentThread.ManagedThreadId];
                    }

                }
            }
        }

        public void CloseSessionConnection()
        {

            if (_sessionCache.ContainsKey(Thread.CurrentThread.ManagedThreadId))
            {
                var session = _sessionCache[Thread.CurrentThread.ManagedThreadId];
                session.Close();
                session.Dispose();
                session = null;
                _maxConnCountEvt.Set();
                lock (this)
                {
                    _sessionCache.Remove(Thread.CurrentThread.ManagedThreadId);
                }

            }
            else
            {
                //throw new Exception("Session Closed more than once , Or Session not open yet");
            }
            //if (_sessionCache.Count >= this.MAX_SESSION_COUNT)
            //{
            //    var toRemove = _sessionCache.Where(a => a.Value.LastChangedTime + _timeOut < DateTime.Now).Select(b => b.Key);
            //    foreach (var key in toRemove)
            //    {
            //        _sessionCache.Remove(key);
            //    }
            //}

        }


    }
}

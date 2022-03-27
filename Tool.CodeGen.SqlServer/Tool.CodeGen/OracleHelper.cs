using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Data;
using System.Configuration; 

namespace DAL
{
    public partial class OracleHelper:HelperBase
    {
        public OracleHelper()
        {
            ConnectionStringLocal = connectionStringLocal;
            Connection = new OracleConnection();
            Command = Connection.CreateCommand();
            Command.CommandTimeout = 3;
        }

        public OracleHelper(int ConnectionStringsIndex)
        {
            if (ConnectionStringsIndex < ConfigurationManager.ConnectionStrings.Count)
            {
                ConnectionStringLocal = ConfigurationManager.ConnectionStrings[ConnectionStringsIndex].ConnectionString;
                Connection = new OracleConnection();
                Command = Connection.CreateCommand();
            }
        }

        public OracleHelper(string ConnectionString)
        {
            this.ConnectionStringLocal = ConnectionString;
            Connection = new OracleConnection();
            Command = Connection.CreateCommand();
        }

        internal override void PrepareCommand(string cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams)
        {
            if (Command.Parameters.Count > 0)
            {
                Command.Parameters.Clear();
            }
            Command.CommandText = cmdText;
            Command.CommandType = cmdType;
            if (cmdParams != null && cmdParams.Length > 0)
            {
                Command.Parameters.AddRange(OracleHelper.CustomParameterCast(cmdParams));
            }
        }

        public DataTable ReadTable(string cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams) 
        {
            PrepareCommand(cmdText, cmdType, cmdParams);
            return ReadTable(Command);
        }

        public Object ExecuteScalar(String cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams) 
        {
            PrepareCommand(cmdText, cmdType, cmdParams);
            return Command.ExecuteScalar();
        }
    }
}

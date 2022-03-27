using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
namespace DAL
{
    public partial class SqlServerHelper:HelperBase
    {
        public SqlServerHelper() 
        {
            ConnectionStringLocal = connectionString;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }

        public SqlServerHelper(int ConnectionStringsIndex)
        {
            ConnectionStringLocal = ConfigurationManager.ConnectionStrings[ConnectionStringsIndex].ConnectionString;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }

        public SqlServerHelper(string ConnectionString)
        {
            this.ConnectionStringLocal = ConnectionString;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }
        internal override void PrepareCommand(string cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams)
        {
            
        }
        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value)
        {
            return AddParameter(ParameterName, type, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type);
            param.Value = value;
            param.Direction = direction;
            Command.Parameters.Add(param);
            return param;
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value)
        {
            return AddParameter(ParameterName, type, size, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type, size);
            param.Direction = direction;
            param.Value = value;
            Command.Parameters.Add(param);
            return param;
        }

        public void AddRangeParameters(SqlParameter[] parameters)
        {
            Command.Parameters.AddRange(parameters);
        }

    }
}

using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Tool.CodeGen
{
    public class MysqlHelper
    {

        public void Open() { }
        public void Close() { }
        private string connStr;

        public MysqlHelper()
        {
            connStr = ConfigurationManager.ConnectionStrings["LocalDBConnectionString"].ConnectionString;
        }

        public DataTable ReadTable(string cmdText, CommandType cmdType, params MySqlParameter[] cmdParams)
        {
            var con = new MySqlConnection(connStr);
            con.Open();
            var cmd = con.CreateCommand();
            if (cmd.Parameters.Count > 0)
            {
                cmd.Parameters.Clear();
            }
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (cmdParams != null && cmdParams.Length > 0)
            {
                cmd.Parameters.AddRange(cmdParams);
            }

            DataTable dt = new DataTable();
            DbDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                int fieldc = reader.FieldCount;
                for (int i = 0; i < fieldc; i++)
                {
                    String colName = reader.GetName(i);
                    if (!dt.Columns.Contains(colName))
                    {
                        DataColumn dc = new DataColumn(colName, reader.GetFieldType(i));
                        dt.Columns.Add(dc);
                    }
                }
                while (reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < fieldc; i++)
                    {
                        dr[i] = reader[i];
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception e) { throw e; }
            finally
            {
                if (reader != null) reader.Close();
                if (con != null) { con.Close(); con.Dispose(); }
            }

        }
    }
}

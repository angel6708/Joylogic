using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data; 
using System.Data.Common;
namespace DAL
{
    public abstract class HelperBase:IDisposable
    {
        #region Fields & Properties
        internal bool isOpen = false;
        private DbConnection _conn;
        private DbCommand _cmd;
        private DbTransaction _trans;
        private Boolean _noCommit;
        private string _connectionStrLocal,
            _connectionStrServer;

        /// <summary>
        /// 服务器连接字符床
        /// </summary>
        public string ConnectionStrServer
        {
            get { return _connectionStrServer; }
            set { _connectionStrServer = value; }
        }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public virtual DbConnection Connection { get { return _conn; } set { _conn = value; } }
        /// <summary>
        /// 数据库T-SQL命令对象
        /// </summary>
        public virtual DbCommand Command { get { return _cmd; } set { _cmd = value; } }
        
        /// <summary>
        /// 本地数据库连接字符串
        /// </summary>
        public string ConnectionStringLocal
        {
            get
            {
                return _connectionStrLocal;
            }
            set
            {
                _connectionStrLocal = value;
            }
        } 
        #endregion

        #region Instance Method
        public int ExecuteStoredProcedure(string StoredProcedureName)
        {
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.CommandText = StoredProcedureName;
            return _cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 对连接对象执行SQL命令
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParams">命令参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNoneQuery(String cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams)
        {
            PrepareCommand(cmdText, cmdType, cmdParams);
            return _cmd.ExecuteNonQuery();
        }

        public Object ExecuteScalar(String cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams)
        {
            PrepareCommand(cmdText, cmdType, cmdParams);
            return _cmd.ExecuteScalar();
        }

        internal abstract void PrepareCommand(String cmdText, CommandType cmdType, params CustomDbParameter[] cmdParams);
        /// <summary>
        /// 打开数据库连接，不使用事务
        /// </summary>
        public virtual void Open()
        {
            Open(false);
        }
        public virtual void BeginTransaction() 
        {
            _trans = _conn.BeginTransaction();
            _cmd.Transaction = _trans;
            _noCommit = true;
        }
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <param name="openTransaction">是否打开事务</param>
        public virtual void Open(Boolean openTransaction)
        {
            _conn.ConnectionString = ConnectionStringLocal;
            try
            {
                _conn.Open();
            }
            catch (Exception ex)
            {
               // DbConnectException dbEx = new DbConnectException(ex.Message, ex, _conn.ConnectionString);
                throw ex;
            }
            if (openTransaction)
            {
                _trans = _conn.BeginTransaction();
                _cmd.Transaction = _trans;
                _noCommit = true;
            }

            isOpen = true;
        }

        /// <summary>
        /// 关闭连接。如果存在事务，则提交事务。
        /// </summary>
        public virtual void Close()
        {
            if (isOpen && _conn != null)
            {
                if (_noCommit)
                {
                    Commit();
                }
                _conn.Close();
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollBack()
        {
            if (_trans != null)
            {
                _trans.Rollback();
                _noCommit = false;
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit() 
        {
            if (_trans != null)
            {
                _trans.Commit();
                _noCommit = false;
            }
        }

        public void Dispose()
        {
            if (_conn != null && _conn.State == ConnectionState.Open) 
            {
                _conn.Close();
                _trans = null;
                _conn = null;
            }
        } 
        #endregion


        #region STATIC
        /// <summary>
        /// 创建命令参数
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="paramName">参数名</param>
        /// <param name="dbtype">类型</param>
        /// <param name="value">值</param>
        /// <returns>新的参数对象</returns>
        public static CustomDbParameter CreateDbParemeter(ParameterDirection direction, string paramName, CustomDbType dbtype, object value)
        {
            return new CustomDbParameter(paramName, value, dbtype, direction);
        }

        /// <summary>
        /// 创建命令参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbtype">类型</param>
        /// <param name="value">值</param>
        /// <returns>新的参数对象</returns>
        public static CustomDbParameter CreateDbParemeter(string paramName, CustomDbType dbtype, object value)
        {
            return CreateDbParemeter(ParameterDirection.Input, paramName, dbtype, value);
        }

        /// <summary>
        /// 创建命令参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbtype">类型</param>
        /// <returns>新的参数对象</returns>
        public static CustomDbParameter CreateDbParemeter(string paramName, CustomDbType dbtype)
        {
            return CreateDbParemeter(ParameterDirection.Input, paramName, dbtype, DBNull.Value);
        }

        /// <summary>
        /// 读表
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static DataTable ReadTable(DbCommand cmd)
        {
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
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        #endregion
    }
}

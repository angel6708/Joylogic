using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.OracleClient;
 

namespace DAL
{
    public partial class OracleHelper:HelperBase
    {
        //定义了一个只读的静态变量，用来获取连接数据库的字符串   "str" == 配置文件中的Name
        public readonly static string connectionStringLocal;
        public readonly static string connectionStringServer;
        static OracleHelper()
        {
            try
            {
                connectionStringLocal = ConfigurationManager.ConnectionStrings["LocalDBConnectionString"].ConnectionString;
                connectionStringServer = ConfigurationManager.ConnectionStrings["ServerDBConnectionString"].ConnectionString;
            }
            catch (Exception ex)
            {
               // LogHelper.Error(LogHelper.CreateSystemMsg("获取连接字符串失败",ex.Message),ex);
                throw;
            }
        }
        public static OracleConnection GetConnection()
        {
            return new OracleConnection(connectionStringLocal);
        }

        #region 01.ExecuteNonQuery操作方法

		/// <summary>
        ///执行一个不需要返回值的OracleCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到OracleCommand的参数集合中
                OracleParameter[] oleParams = new OracleParameter[commandParameters.Length];
                for (Int32 i = 0; i < commandParameters.Length; i++)
                {
                    oleParams[i] = CustomParameterCast(commandParameters[i]);
                }
                PrepareCommand(cmd, conn, null, cmdType, cmdText, oleParams);
                int val = cmd.ExecuteNonQuery();
                for (Int32 i = 0; i < commandParameters.Length; i++)
                {
                    if (commandParameters[i].Direction != ParameterDirection.Input)
                    {
                        commandParameters[i].Value = oleParams[i].Value;
                    }
                }
                return val;
            }
            
        }

        /// <summary>
        ///执行一个不需要返回值的OracleCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameter)
        {
            return ExecuteNonQuery(connectionStringLocal,cmdType, cmdText, commandParameter);
        }

        /// <summary>
        ///存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQueryProcedure(string procName, params CustomDbParameter[] commandParameter)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, procName, commandParameter);
        }

        /// <summary>
        ///Sql语句专用
        /// </summary>
        /// <param name="cmdText">T_Sql语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, params CustomDbParameter[] commandParameter)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText, commandParameter);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或T-SQL语句</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回影响的行数</returns>
        private static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameter)
        {
            OracleCommand cmd = new OracleCommand();
            OracleParameter[] oleParams = new OracleParameter[commandParameter.Length];
            for (Int32 i = 0; i < commandParameter.Length; i++) 
            {
                oleParams[i] = CustomParameterCast(commandParameter[i]);
            }
            PrepareCommand(cmd, connection, null, cmdType, cmdText, oleParams);
            int val = cmd.ExecuteNonQuery();
            for (Int32 i = 0; i < commandParameter.Length; i++)
            {
                if (commandParameter[i].Direction != ParameterDirection.Input) 
                {
                    commandParameter[i].Value = oleParams[i].Value;
                }
            }
            cmd.Parameters.Clear();

            return val;

        }

	    #endregion

        #region 02.ExecuteReader操作方法
		
        /// <summary>
        ///执行一个不需要返回值的OracleCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="connection">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameter">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回包含结果集的OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string connection, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameter)
        {
            OracleCommand cmd=new OracleCommand ();
            OracleConnection conn=new OracleConnection (connection);
            try 
	        {	        
		        PrepareCommand(cmd,conn,null,cmdType,cmdText,commandParameter);
                OracleDataReader rdr=cmd.ExecuteReader(CommandBehavior.CloseConnection);

                cmd.Parameters.Clear();

                return rdr;
	        }
	        catch (Exception)
	        {
		        conn.Close();
		        throw;
	        }
        }

        /// <summary>
        /// 执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <param name="conn">一个有效的数据库连接对象</param>
        /// <param name="cmdText">存储过程名称或T-SQL语句</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回包含结果集的OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(OracleConnection conn, string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ExecuteReader(connectionStringLocal, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// T-SQL语句专用--执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <param name="cmdText">T-SQL语句</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回包含结果集的OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ExecuteReader(connectionStringLocal, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 存储过程专用--执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回包含结果集的OracleDataReader</returns>
        public static OracleDataReader ExecuteReaderProc(string StoredProcedureName, params CustomDbParameter[] commandParameters)
        {
            return ExecuteReader(connectionStringLocal, CommandType.StoredProcedure, StoredProcedureName, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程名称或T-SQL语句</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回包含结果集的OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ExecuteReader(connectionStringLocal, cmdType, cmdText, commandParameters);
        }
         
	    #endregion

        #region 03.ExecuteScalar方法

        /// <summary>
        /// 执行返回的第一条记录的第一列的SqlCommand 通过一个已经存在的数据库连接
        /// 使用参数数组提供参数 
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 返回第一行的第一列Sql语句专用
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="cmdText">T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalar(string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ExecuteScalar(connectionStringLocal, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 返回第一行的第一列存储过程专用
        /// </summary>
        /// <param name="StoredProcedureName">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalarProc(string StoredProcedureName, params CustomDbParameter[] commandParameters)
        {
            return ExecuteScalar(connectionStringLocal, CommandType.StoredProcedure, StoredProcedureName, commandParameters);
        }

        /// <summary>
        /// 返回第一行的第一列
        /// </summary>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ExecuteScalar(connectionStringLocal, cmdType, cmdText, commandParameters);
        }
        /// <summary>
        /// 执行返回的第一条记录的第一列的OracleCommand 通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        private static object ExecuteScalar(OracleConnection connection, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

	#endregion

        #region 04.ReadTable方法

        /// <summary>
        /// 执行一条返回结果集的OracleCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="connecttionString">一个现有的数据库连接</param>
        /// <param name="cmdTye">OracleCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTable ReadTable(string connectionString, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameter)
        {
            using(OracleConnection conn=new OracleConnection (connectionString))
	        {
                try
                {
                    conn.Open();
                }
                catch(Exception ex)
                {
                    //DbConnectException dbEx = new DbConnectException(ex.Message, ex,connectionString);
                    throw ex;
                }
                return ReadTable(conn,cmdType,cmdText,commandParameter);
	        }
        }
        /// <summary>
        /// Sql语句专用
        /// </summary>
        /// <param name="cmdText"> T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTable ReadTable(string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ReadTable(CommandType.Text, cmdText, commandParameters);
        }
        /// <summary>
        /// 存储过程专用
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTable ReadTableProcedure(string procName, params CustomDbParameter[] commandParameters)
        {
            return ReadTable(CommandType.StoredProcedure, procName, commandParameters);
        }
        /// <summary>
        /// 执行一条返回结果集的OracleCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="cmdTye">OracleCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTable ReadTable(CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            return ReadTable(connectionStringLocal, cmdType, cmdText, commandParameters);
        }


        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或T-SQL语句</param>
        /// <param name="commandParameters">OracleParamter参数数组</param>
        /// <returns>返回的是表的集合</returns>
        private static DataTable ReadTable(OracleConnection connection, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameter)
        {
            OracleCommand cmd=new OracleCommand ();
            PrepareCommand(cmd, connection,null,cmdType,cmdText,commandParameter);
            DataTable dt=HelperBase.ReadTable(cmd);
            cmd.Parameters.Clear();
            return dt;
        }
        /// <summary>
        /// 执行带事务的OracleCommand(指定参数).
        /// </summary>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或T-SQL语句</param>
        /// <param name="commandParameters">OracleCommand参数数组</param>
        /// <returns>返回的是表的集合</returns>
        private static DataTable ReadTable(OracleTransaction transaction, CommandType cmdType, string cmdText, params CustomDbParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, cmdType, cmdText, commandParameters);
            DataTable dt = HelperBase.ReadTable(cmd);
            cmd.Parameters.Clear();
            return dt;
        }
 
	    #endregion
   

        #region 05.为执行命令准备参数  +private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">OracleCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, CustomDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                   // DbConnectException dbEx = new DbConnectException(ex.Message, ex, conn.ConnectionString);
                    throw ex;
                }
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (CustomDbParameter parm in cmdParms)
                {
                    OracleParameter oPara = CustomParameterCast(parm);
                    cmd.Parameters.Add(oPara);
                }

            }
        }
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    //DbConnectException dbEx = new DbConnectException(ex.Message, ex, conn.ConnectionString);
                    throw ex;
                }
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            cmd.Parameters.AddRange(cmdParms);
        }
        #endregion

        #region 07.检查是否存在

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <returns>bool结果</returns>
        public static bool Exists(string strSql)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(connectionStringLocal, CommandType.Text, strSql, null));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>bool结果</returns>
        public static bool Exists(string strSql, params CustomDbParameter[] cmdParms)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(connectionStringLocal, CommandType.Text, strSql, cmdParms));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        /// <summary>
        /// 转换自定义的字段数据类型
        /// </summary>
        /// <param name="cType">自定义的字段数据类型</param>
        /// <returns>Oracle数据库的字段数据类型</returns>
        private static OracleType CustomDbTypeCast(CustomDbType cType) 
        {
            switch (cType) 
            {
                case CustomDbType.DateTime:
                    return OracleType.DateTime;
                case CustomDbType.Single:
                    return OracleType.Float;
                case CustomDbType.Int16:
                    return OracleType.Int16;
                case CustomDbType.Int32:
                    return OracleType.Int32;
                case CustomDbType.Boolean:
                    return OracleType.Number;
                case CustomDbType.Guid:
                case CustomDbType.String:
                default:
                    return OracleType.VarChar;
            }
        }

        /// <summary>
        /// 转换自定义的命令参数
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        private static OracleParameter CustomParameterCast(CustomDbParameter para) 
        {
            OracleParameter oPara = new OracleParameter();
            oPara.OracleType = CustomDbTypeCast(para.DataType);
            oPara.ParameterName = para.Name;
            if (para.Value == null) 
            {
                oPara.Value = DBNull.Value;
            }
            else if (para.DataType == CustomDbType.Guid)
            {
                oPara.Value = para.Value.ToString();
            }
            else if (para.DataType == CustomDbType.Boolean) 
            {
                if (para.Value.ToString() == Boolean.TrueString)
                {
                    oPara.Value = 1;
                }
                else 
                {
                    oPara.Value = 0;
                }
            }
            else
            {
                oPara.Value = para.Value;
            }
            oPara.Direction = para.Direction;
            if (para.Length > 0)
            {
                oPara.Size = para.Length;
            }

            return oPara;
        }
        private static OracleParameter[] CustomParameterCast(params CustomDbParameter[] pArrayCustom)
        {
            OracleParameter[] pArrayOle = new OracleParameter[pArrayCustom.Length];
            for (Int32 i = 0; i < pArrayCustom.Length; i++) 
            {
                pArrayOle[i] = CustomParameterCast(pArrayCustom[i]);
            }
            return pArrayOle;
        }
    }
}

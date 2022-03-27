using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Data.Access.Interface;
using Data.Models;

namespace Data.Access
{


    public abstract class BaseBLL<T> : IBaseBLL<T> where T : IModel
    {
         
        public BaseBLL() { }


        #region SqlBuilding

        public string BuildBaseSql()
        {
            StringBuilder selectValues = new StringBuilder();

            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                selectValues.AppendFormat(" `{2}`.`{0}` `{1}`,", c.Key, c.Value, TableInfo.FromType(typeof(T)).TableName);
            }

            selectValues.Remove(selectValues.Length - 1, 1);
            return string.Format("SELECT {1} FROM `{0}` ", TableInfo.FromType(typeof(T)).TableName, selectValues);
        }


        protected string BuildPageCountSql(string where)
        {
            var sql = string.Format("SELECT COUNT(*) AS DATA_COUNT FROM {0} {1}", TableInfo.FromType(typeof(T)).TableName, where);
            return sql;
        }

        protected string BuildGetSql()
        {

            StringBuilder selectValues = new StringBuilder();
            string keyWhere = string.Format("`{0}` = @{1}_", TableInfo.FromType(typeof(T)).KeyColumn, TableInfo.FromType(typeof(T)).ColumnPropMaps[TableInfo.FromType(typeof(T)).KeyColumn]);
            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                selectValues.AppendFormat("`{0}` `{1}`,", c.Key, c.Value);
            }

            selectValues.Remove(selectValues.Length - 1, 1);
            return string.Format("SELECT {1} FROM `{0}` WHERE {2}", TableInfo.FromType(typeof(T)).TableName, selectValues, keyWhere);

        }

        protected string BuildInsertSql()
        {

            StringBuilder cols = new StringBuilder();
            StringBuilder parms = new StringBuilder();
            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                cols.AppendFormat("`{0}`,", c.Key);
                parms.AppendFormat("@{0}_,", c.Value);
            }

            cols.Remove(cols.Length - 1, 1);
            parms.Remove(parms.Length - 1, 1);
            return string.Format("INSERT INTO `{0}` ({1}) VALUES ({2})", TableInfo.FromType(typeof(T)).TableName, cols, parms);
        }

        protected string BuildUpdateSql()
        {
            StringBuilder setters = new StringBuilder();
            string keyWhere = string.Format("`{0}` = @{1}_", TableInfo.FromType(typeof(T)).KeyColumn, TableInfo.FromType(typeof(T)).ColumnPropMaps[TableInfo.FromType(typeof(T)).KeyColumn]);
            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                if (c.Key != TableInfo.FromType(typeof(T)).KeyColumn)
                    setters.AppendFormat("`{0}` = @{1}_,", c.Key, c.Value);
            }
            setters.Remove(setters.Length - 1, 1);

            return string.Format("UPDATE `{0}` SET {1} WHERE {2}", TableInfo.FromType(typeof(T)).TableName, setters, keyWhere);
        }

        protected string BuildDeleteSql()
        {
            string keyWhere = string.Format("`{0}` = @{1}_", TableInfo.FromType(typeof(T)).KeyColumn, TableInfo.FromType(typeof(T)).ColumnPropMaps[TableInfo.FromType(typeof(T)).KeyColumn]);
            return string.Format("DELETE FROM `{0}` WHERE {1}", TableInfo.FromType(typeof(T)).TableName, keyWhere);
        }


        protected string BuildGetCurrentSql()
        {

            StringBuilder selectValues = new StringBuilder();
            string keyWhere = string.Format("`{0}` = @{1}_  AND update_time IS NULL ", "KEY", "Key");
            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                selectValues.AppendFormat("`{0}` `{1}`,", c.Key, c.Value);
            }

            selectValues.Remove(selectValues.Length - 1, 1);
            return string.Format("SELECT {1} FROM `{0}` WHERE {2}", TableInfo.FromType(typeof(T)).TableName, selectValues, keyWhere);

        }

        public static string BuildReturnValues(string aliasModelName)
        {
            StringBuilder selectValues = new StringBuilder();
            foreach (var c in TableInfo.FromType(typeof(T)).ColumnPropMaps)
            {
                selectValues.AppendFormat("{2}.`{0}` `{1}`,", c.Key, c.Value, aliasModelName);
            }
            selectValues.Remove(selectValues.Length - 1, 1);
            return selectValues.ToString();

        }


        #endregion

        #region Basic Db access

        public virtual List<T> ListModels(int pageSize, int index)
        {

            string whereSql = string.Format(" order by `KEY` asc limit {0},{1}", index, pageSize);
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                whereSql = " where update_time is null " + whereSql;
            }
            var sql = this.BuildBaseSql() + whereSql;

            // var sql = this.BuildPageSql(whereSql, index, pageSize);
            return SqlMapper.Query<T>(ConnectionFactory.Current.GetSessionConnection(), sql).ToList();

        }

        public virtual int GetModelCount()
        {
            string whereSql = " order by `KEY` asc ";
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                whereSql = " where update_time is null " + whereSql;
            }
            var sql = this.BuildPageCountSql(whereSql);
            var command = ConnectionFactory.Current.GetSessionConnection().CreateCommand();
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Delete(T obj)
        {
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                obj.UpdateTime = DateTime.Now;
                SqlMapper.Execute(ConnectionFactory.Current.GetSessionConnection(), this.BuildUpdateSql(), obj.GetParam());
            }
            else
            {
                SqlMapper.Execute(ConnectionFactory.Current.GetSessionConnection(), this.BuildDeleteSql(), obj.GetParam());
            }
        }

        public int Save(T obj)
        {
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                obj.UpdateTime = null;
            }
            return SqlMapper.Execute(ConnectionFactory.Current.GetSessionConnection(), BuildInsertSql(), obj.GetParam());

        }

        public void Update(T obj)
        {
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                var last = this.Get(obj);
                this.Delete(last);
                this.Save(obj);
            }
            SqlMapper.Execute(ConnectionFactory.Current.GetSessionConnection(), BuildUpdateSql(), obj.GetParam());
        }

        public T Get(T obj)
        {
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                return SqlMapper.Query<T>(ConnectionFactory.Current.GetSessionConnection(), BuildGetCurrentSql(), obj.GetParam()).FirstOrDefault();
            }
            else
            {
                return SqlMapper.Query<T>(ConnectionFactory.Current.GetSessionConnection(), BuildGetSql(), obj.GetParam()).FirstOrDefault();
            }
        }

        public T GetBySourceId(string sourceId)
        {
            string where = " where `source_id` = @SourceId_";
            if (TableInfo.FromType(typeof(T)).IsSnapshot)
            {
                where = " where `source_id` = @SourceId_  AND update_time IS NULL ";
            }

            var sql = BuildBaseSql() + where;
            return SqlMapper.Query<T>(ConnectionFactory.Current.GetSessionConnection(), sql, new { SourceId_ = sourceId }).FirstOrDefault();

        }

        public List<T> GetHistoryData(DateTime from, DateTime to, Guid? deviceKey)
        {
            var sql = this.BuildBaseSql() + " WHERE CREATE_TIME BETWEEN @FROM_ AND @TO_ ";
            if (TableInfo.FromType(typeof(T)).ColumnPropMaps.Keys.Contains("DEVICE_KEY") && deviceKey != null)
            {
                sql = sql + " AND DEVICE_KEY = @DEVICE_KEY_ ";
            }
            return SqlMapper.Query<T>(ConnectionFactory.Current.GetSessionConnection(), sql, new
            {
                FROM_ = from,
                TO_ = to,
                DEVICE_KEY_ = deviceKey.GetValueOrDefault()
            }).ToList();
        }

        #endregion
    }
}

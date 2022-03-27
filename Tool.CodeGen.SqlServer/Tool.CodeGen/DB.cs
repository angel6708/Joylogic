using DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    public class DB
    {
        private SqlServerHelper _helper = new SqlServerHelper();

        public List<Schema> GetAllSchemas()
        {

            return new List<Schema>(new[] { new Schema { Name = "admed", Refs = new List<Ref>(), Tables = new List<Table>(), Views = new List<Table>() } });

            List<Schema> result = new List<Schema>();
            _helper.Open();
            _helper.PrepareCommand(Const.SCHEMA_SQL, CommandType.Text, new CustomDbParameter[0]);
            DataTable dt = SqlServerHelper.ReadTable(_helper.Command);

            foreach (DataRow row in dt.Rows)
            {
                Schema s = new Schema();
                s.Name = row["USERNAME"] as string;
                result.Add(s);
            }
            _helper.Close();
            return result;
        }

        public List<Table> GetAllTabs(Schema s)
        {
            _helper.Open();
            List<Table> result = new List<Table>();
            DataTable dt = SqlServerHelper.GetTable(CommandType.Text, Const.SCHEMA_TABS, new SqlParameter[0]);
            // DataTable dt = SqlServerHelper.ReadTable(_helper.Command);

            foreach (DataRow row in dt.Rows)
            {
                Table t = new Table();
                t.Name = row["TABLE_NAME"] as string;
                if (t.Name.IndexOf("_BASE") > 0)
                {
                    t.isSnapshotBase = true;
                }
                t.IsView = false;
                TextInfo text = new CultureInfo("en").TextInfo;
                // t.AName = text.ToTitleCase(t.Name.ToLower()).Replace("_", "");
                var name = t.Name;
                if (name.IndexOf('.') > 0)
                {
                    name = name.Substring(name.IndexOf('.') + 1);
                }
                t.AName = text.ToTitleCase(name.ToLower()).Replace("_", "");

                result.Add(t);
            }
            _helper.Close();

            return result;
        }

        public List<Table> GetAllViews(Schema s)
        {
            return new List<Table>();
        }

        public List<Ref> GetAllRefs(Schema s)
        {
            List<Ref> result = new List<Ref>();
            return result;
            _helper.Open();

            //DataTable dt = _helper.ReadTable(Const.TAB_REF, System.Data.CommandType.Text, new MySqlParameter[] { new MySqlParameter("owner", s.Name) });
            DataTable dt = null;
            foreach (DataRow row in dt.Rows)
            {
                var tab = s.Tables.Where(a => a.Name == (row["ONETAB"] as string)).FirstOrDefault();
                if (tab.isSnapshotBase)
                {
                    if (row["MANYCOL"] as string == "KEY")
                    {
                        tab.SnapshotTableName = row["MANYTAB"] as string;
                        var stab = s.Tables.Where(a => a.Name == tab.SnapshotTableName).FirstOrDefault();
                        stab.IsSnapshot = true;
                        stab.SnaoshotTableBaseName = tab.Name;
                    }
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                var otab = s.Tables.Where(a => a.Name == (row["ONETAB"] as string)).FirstOrDefault();
                var mtab = s.Tables.Where(a => a.Name == (row["MANYTAB"] as string)).FirstOrDefault();

                if (otab.isSnapshotBase && (row["MANYTAB"] as string) == otab.SnapshotTableName) { continue; }

                Ref r = new Ref();
                r.OneTabCol = row["ONECOL"] as string;
                r.OneTable = row["ONETAB"] as string;
                r.ManyTabCol = row["MANYCOL"] as string;
                r.ManyTable = row["MANYTAB"] as string;
                r.OneTableEntity = otab;
                if (otab.isSnapshotBase)
                {
                    r.OneTable = otab.SnapshotTableName;
                    r.OneTableEntity = s.Tables.Where(a => a.Name == otab.SnapshotTableName).FirstOrDefault();
                }



                TextInfo text = new CultureInfo("en").TextInfo;
                r.AName = text.ToTitleCase(r.OneTable.ToLower()).Replace("_", "");
                result.Add(r);
            }
            _helper.Close();

            return result;
        }

        public List<Column> GetAllCols(Schema s, Table t)
        {
            _helper.Open();
            List<Column> result = new List<Column>();
            DataTable dt = SqlServerHelper.GetTable(SqlServerHelper.connectionString, CommandType.Text, Const.TAB_DESC, new SqlParameter[]
            {
                new SqlParameter("table_name", t.Name.Split('.')[1])
            });



            foreach (DataRow row in dt.Rows)
            {
                Column c = new Column();
                c.ColLen = (int)(Int16)row["DATA_LENGTH"];
                c.DbType = row["DATA_TYPE"] as string;
                c.IsNullable = row["NULLABLE"].ToString() == "YES";
                c.IsPK = row["CONSTRAINT_TYPE"].ToString() == "PRI";
                c.Name = row["COLUMN_NAME"] as string;
                c.Desc = row["COLUMN_COMMENT"] as string;
                TextInfo text = new CultureInfo("en").TextInfo;

                if (result.Where(a => a.Name == c.Name).Count() > 0)
                {
                    continue;
                }

                string name = c.Name;
                if (name.IndexOf('.') > 0)
                {
                    name = name.Substring(name.IndexOf('.') + 1);
                }
                c.AName = text.ToTitleCase(name.ToLower()).Replace("_", "");


                if (c.LanType == "enum")
                {
                    //t.Enums.Add(c.AName);
                }

                result.Add(c);
            }
            _helper.Close();

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BlitzkriegSoftware.AdoSqlHelper;

namespace Blitz.SqlExtract2SeedData.Libs
{
    public static class SqlExtractor
    {

        public static Models.TableInfo ParseTableName(Models.Options options)
        {
            var ti = new Models.TableInfo();
            if (options != null)
            {
                var s = options.Table.Replace("[", "").Replace("]", "");
                int i = s.IndexOf('.');
                if (i < 0)
                {
                    ti.TableName = s;
                }
                else
                {
                    ti.Schema = s.Substring(0, i);
                    ti.TableName = s.Substring(i + 1);
                }
            }
            return ti;
        }

        public static bool HasIdentity(Models.Options options)
        {
            if (options != null)
            {
                var ti = ParseTableName(options);
                return HasIdentity(options.ConnectionString, ti);
            }
            else
            {
                return false;
            }
        }

        public static bool HasIdentity(string connectionString, Models.TableInfo ti)
        {
            if (ti != null)
            {
                string SQL = "SELECT OBJECT_NAME(OBJECT_ID) AS TABLENAME, NAME AS COLUMNNAME, SEED_VALUE, INCREMENT_VALUE, LAST_VALUE, IS_NOT_FOR_REPLICATION FROM SYS.IDENTITY_COLUMNS WHERE OBJECT_NAME(OBJECT_ID) = '{0}' AND OBJECT_SCHEMA_NAME(object_id) = '{1}'";
                SQL = string.Format(SQL, ti.TableName, ti.Schema);
                var dt = SqlHelper.ExecuteSqlWithParametersToDataTable(connectionString, SQL, null);
                return SqlHelper.HasRows(dt);
            }
            else
            {
                return false;
            }
        }

        public static void Extract(Models.Options options)
        {
            if (options == null) return;

            var ti = ParseTableName(options);
            var isIdentity = HasIdentity(options.ConnectionString, ti);

            var filename = $"{ti.Schema}-{ti.TableName}-SeedData.sql";
            filename = Path.Combine(Directory.GetCurrentDirectory(), filename);

            var sb = new StringBuilder();

            sb.Append("select ");
            if (options.Top > 0)
            {
                sb.Append($"top ({options.Top}) ");
            }

            sb.Append($"* from {ti.ToString()} ");

            if (!string.IsNullOrWhiteSpace(options.Where))
            {
                sb.Append(options.Where);
                sb.Append(" ");
            }

            if (!string.IsNullOrWhiteSpace(options.OrderBy))
            {
                sb.Append(options.OrderBy);
                sb.Append(" ");
            }

            sb.Append(";");

            var sql = sb.ToString();

            var dt = SqlHelper.ExecuteSqlWithParametersToDataTable(options.ConnectionString, sql, null);
            if (SqlHelper.HasRows(dt))
            {
                var clist = new StringBuilder();
                clist.Append("( ");
                foreach (DataColumn dc in dt.Columns)
                {
                    clist.Append($"[{dc.ColumnName}], ");
                }

                var columns = clist.ToString().Trim();
                columns = columns.Substring(0, columns.Length - 1);
                columns += " )";

                if (File.Exists(filename)) File.Delete(filename);

                using (var file = new System.IO.StreamWriter(filename))
                {
                    if (isIdentity)
                    {
                        file.WriteLine($"SET IDENTITY_INSERT {ti.ToString()} ON");
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        file.Write($"INSERT INTO {ti.ToString()} {columns} VALUES (");

                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            var tname = dr.ItemArray[i].GetType().Name;
                            switch (tname)
                            {
                                case "String":
                                case "System.String":
                                    file.Write("'");
                                    file.Write(FixQuote(dr.ItemArray[i].ToString()));
                                    file.Write("'");
                                    if (i < (dr.ItemArray.Length - 1)) file.Write(", ");
                                    break;

                                case "Boolean":
                                case "System.Boolean":
                                    var fl = Boolean.Parse(dr.ItemArray[i].ToString());
                                    int val = (fl) ? 1 : 0;
                                    file.Write(val);
                                    break;

                                default:
                                    file.Write(dr.ItemArray[i]);
                                    if (i < (dr.ItemArray.Length - 1)) file.Write(", ");
                                    break;
                            }
                        }

                        file.WriteLine(");");
                    }

                    if (isIdentity)
                    {
                        file.WriteLine($"SET IDENTITY_INSERT {ti.ToString()} OFF");
                    }
                }
            }
            else
            {
                Console.Error.WriteLine($"No Rows Returned for: {sql}");
            }

        }

        public static string FixQuote(string s)
        {
            return !string.IsNullOrEmpty(s) ? s.Replace("'", "`") : string.Empty;
        }

    }
}
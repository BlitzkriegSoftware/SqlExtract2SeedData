using System;
using System.IO;
using System.Text;
using System.Data;
using BlitzkriegSoftware.AdoSqlHelper;

namespace Blitz.SqlExtract2SeedData.Libs
{
    /// <summary>
    /// SQL Extractor
    /// </summary>
    public static class SqlExtractor
    {

        /// <summary>
        /// Parses and normalizes table name
        /// </summary>
        /// <param name="options">(options)</param>
        /// <returns>Table and Schema</returns>
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

        /// <summary>
        /// Does the table have an identity column
        /// </summary>
        /// <param name="options">(options)</param>
        /// <returns>True if so</returns>
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

        /// <summary>
        /// Does the table have an identity column
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="ti">TableInfo</param>
        /// <returns>True if so</returns>
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

        /// <summary>
        /// The Extraction Engine
        /// </summary>
        /// <param name="options">(options)</param>
        public static void Extract(Models.Options options)
        {
            if (options == null) return;

            var ti = ParseTableName(options);
            var isIdentity = HasIdentity(options.ConnectionString, ti);


            if (options.Verbose)
            {
                Console.WriteLine($"{ti.ToString()} has identity {isIdentity}");
            }

            var filename = $"{ti.Schema}-{ti.TableName}-SeedData.sql";
            if (options.AsCsv) filename = Path.ChangeExtension(filename, "csv");
            filename = Path.Combine(Directory.GetCurrentDirectory(), filename);
            if (File.Exists(filename)) File.Delete(filename);

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

            if(options.Verbose)
            {
                Console.WriteLine($"SQL Query: {sql}");
            }

            if (options.AsCsv)
            {
                isIdentity = false;
            }

            var dt = SqlHelper.ExecuteSqlWithParametersToDataTable(options.ConnectionString, sql, null);
            if (SqlHelper.HasRows(dt))
            {
                var columns = ColumnsList(dt, !options.AsCsv);

                // New style using
                using var file = new StreamWriter(filename);

                if (isIdentity)
                {
                    file.WriteLine($"SET IDENTITY_INSERT {ti.ToString()} ON");
                }

                if(options.AsCsv)
                {
                    file.WriteLine(columns);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if(options.AsCsv)
                    {
                        WriteRowCsv(file, dr);
                    } else
                    {
                        WriteRowSql(file, dr, columns, ti);
                    }
                }

                if (isIdentity)
                {
                    file.WriteLine($"SET IDENTITY_INSERT {ti.ToString()} OFF");
                }
            }
            else
            {
                Console.Error.WriteLine($"No Rows Returned for: {sql}");
            }

            Console.WriteLine($"Written to: {filename}");
        }

        /// <summary>
        /// Creates a formatted column list
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="asSql"></param>
        /// <returns></returns>
        public static string ColumnsList(DataTable dt, bool asSql)
        {
            if (dt == null) throw new ArgumentNullException(nameof(dt), "There must be a datatable"); 

            var clist = new StringBuilder();
            if(asSql) clist.Append("( ");
            
            foreach (DataColumn dc in dt.Columns)
            {
                if(asSql)
                {
                    clist.Append("[");
                }

                var name = dc.ColumnName;
                if(!asSql)
                {
                    name = name.Replace(' ', '_');
                }

                clist.Append($"{name}");
                
                if(asSql)
                {
                    clist.Append("],"); 
                    clist.Append(" ");
                } else
                {
                    clist.Append(",");
                }

            }

            var columns = clist.ToString().Trim();
            columns = columns[0..^1];

            if(asSql)
            {
                columns += " )";
            }

            return columns;
                
        }

        /// <summary>
        /// Write one row as CSV
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dr"></param>
        public static void WriteRowCsv(StreamWriter file, DataRow dr)
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "File stream writer must not be null");
            if (dr == null) throw new ArgumentNullException(nameof(dr), "DataRow must not be null");

            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                var tname = dr.ItemArray[i].GetType().Name;
                switch (tname)
                {
                    case "String":
                    case "System.String":
                        file.Write('"');
                        file.Write(FixQuote(dr.ItemArray[i].ToString()));
                        file.Write('"');
                        break;

                    default:
                        file.Write(dr.ItemArray[i]);
                        break;
                }

                if (i < (dr.ItemArray.Length - 1)) file.Write(",");
            }

            file.WriteLine("");

        }

        /// <summary>
        /// Write one row as SQL Insert
        /// </summary>
        /// <param name="file">StreamWriter</param>
        /// <param name="dr">DataRow</param>
        /// <param name="columns">Columns</param>
        /// <param name="ti">TableInfo</param>
        public static void WriteRowSql(StreamWriter file, DataRow dr, string columns, Models.TableInfo ti) 
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "File stream writer must not be null");
            if (dr == null) throw new ArgumentNullException(nameof(dr), "DataRow must not be null");
            if (ti == null) throw new ArgumentNullException(nameof(ti), "Table info should not be null");

            file.Write($"INSERT INTO {ti.ToString()} {columns} VALUES (");

            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                var tname = dr.ItemArray[i].GetType().Name;
                switch (tname)
                {
                    case "String":
                    case "System.String":
                        file.Write("'");
                        file.Write(FixApostrophe(dr.ItemArray[i].ToString()));
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

        /// <summary>
        /// Fix embedded <c>Apostrophe</c> in data
        /// </summary>
        /// <param name="s">input text</param>
        /// <returns>output text</returns>
        public static string FixApostrophe(string s)
        {
            return !string.IsNullOrEmpty(s) ? s.Replace("'", "`") : string.Empty;
        }

        /// <summary>
        /// Fix Quote in data
        /// </summary>
        /// <param name="s">input text</param>
        /// <returns>output text</returns>
        public static string FixQuote(string s)
        {
            return !string.IsNullOrEmpty(s) ? s.Replace("\"", "\"\"").Trim() : string.Empty;
        }
    }
}
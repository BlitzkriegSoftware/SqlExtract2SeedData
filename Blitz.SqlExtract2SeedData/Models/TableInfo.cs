using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.SqlExtract2SeedData.Models
{
    /// <summary>
    /// Table Info
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; } = "dbo";

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[{this.Schema}].[{this.TableName}]";
        }
    }
}

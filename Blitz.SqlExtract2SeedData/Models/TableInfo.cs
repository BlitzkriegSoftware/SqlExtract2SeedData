namespace Blitz.SqlExtract2SeedData.Models
{
    /// <summary>
    /// Table Info
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Schema, with a default of <c>dbo</c>
        /// </summary>
        public string Schema { get; set; } = "dbo";

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Returns normalized table name and schema
        /// </summary>
        /// <returns>See above</returns>
        public override string ToString()
        {
            return $"[{this.Schema}].[{this.TableName}]";
        }
    }
}

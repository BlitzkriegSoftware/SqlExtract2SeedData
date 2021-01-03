using CommandLine;

namespace Blitz.SqlExtract2SeedData.Models
{
    /// <summary>
    /// Command line options
    /// <para>See: https://github.com/commandlineparser/commandline</para>
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Verbose Output
        /// </summary>
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Json Configuration File Path
        /// </summary>
        [Option('c', "connectstring", Required = true, HelpText = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// SQL Table Name
        /// </summary>
        [Option('t', "table", Required = true, HelpText = "SQL Table To Extract Data From")]
        public string Table { get; set; } = string.Empty;

        /// <summary>
        /// SQL Order By
        /// </summary>
        [Option('o', "orderby", Required = false, HelpText = "(optional) Order By Clause in the form of \"order by column1, column2\"")]
        public string OrderBy { get; set; } = string.Empty;

        /// <summary>
        /// SQL Order By
        /// </summary>
        [Option('w', "where", Required = false, HelpText = "(optional) Where  Clausein the form of \"where (column1 = 3)\"")]

        public string Where { get; set; } = string.Empty;

        /// <summary>
        /// SQL Order By
        /// </summary>
        [Option('n', "ntop", Required = false, HelpText = "(optional) Top N Rows")]
        public int Top { get; set; } = -1;

        /// <summary>
        /// Emit TSV Instead
        /// </summary>
        [Option('a', "ascsv", Required=false, HelpText ="Emit Tab Separated Values (.csv) Instead of Seed Data")]
        public bool AsCsv { get; set; } = false;
    }
}

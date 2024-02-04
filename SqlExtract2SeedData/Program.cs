using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Blitz.SqlExtract2SeedData.Libs;
using Blitz.SqlExtract2SeedData.Models;

using CommandLine;

namespace Blitz.SqlExtract2SeedData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{Program.ProgramMetadata}");

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed<CommandLineOptions>(o =>
                   {
                       try
                       {
                           Libs.SqlExtractor.Extract(o);
                       } catch (SqlException ex)
                       {
                           Console.Error.WriteLine($"Unable to extract data: {ex.Message}\nIs your connection string correct? Check all the parameters as well.");
                       }

                   })
                   .WithNotParsed(HandleParseError);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("The command line parameters are incorrect:");
            foreach(var e in errs)
            {
                Console.WriteLine($"\t{e.Tag}");
            }
        }

        private static Models.BlitzAssemblyVersionMetadata _blitzassemblyversionmetadata;

        /// <summary>
        /// Semantic Version, etc from Assembly Metadata
        /// </summary>
        public static Models.BlitzAssemblyVersionMetadata ProgramMetadata
        {
            get
            {
                if (_blitzassemblyversionmetadata == null)
                {
                    _blitzassemblyversionmetadata = new Models.BlitzAssemblyVersionMetadata();
                    var assembly = typeof(Program).Assembly;
                    foreach (var attribute in assembly.GetCustomAttributesData())
                    {
                        if (!attribute.TryParse(out string value))
                        {
                            value = string.Empty;
                        }
                        var name = attribute.AttributeType.Name;
                        System.Diagnostics.Trace.WriteLine($"{name}, {value}");
                        _blitzassemblyversionmetadata.PropertySet(name, value);
                    }
                }
                return _blitzassemblyversionmetadata;
            }
        }

    }
}

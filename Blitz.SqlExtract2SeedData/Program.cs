using Blitz.SqlExtract2SeedData.Libs;
using Blitz.SqlExtract2SeedData.Models;
using CommandLine;
using System;
using System.Collections.Generic;

namespace Blitz.SqlExtract2SeedData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{Program.ProgramMetadata.ToString()}");

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Libs.SqlExtractor.Extract(o);
                   })
                   .WithNotParsed(HandleParseError);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach(var e in errs)
            {
                Console.WriteLine($" {e.Tag}");
            }
        }

        private static Models.BlitzAssemblyVersionMetadata _blitzassemblyversionmetadata = null;

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

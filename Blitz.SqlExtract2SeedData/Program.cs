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
    }
}

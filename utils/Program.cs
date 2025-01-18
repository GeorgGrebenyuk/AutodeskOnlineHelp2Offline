using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AD_HelpGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            CommandLine.Parser.Default.ParseArguments<AD_HelpGen_Config>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
#else
            AD_HelpGen_Config options_test = new AD_HelpGen_Config()
            {
                //WorkMode = "GetLinks",
                //TocTreePath = @"E:\Temp\AD_C3D_Docs\api\toctree.json"

                //WorkMode = "ValidateLinks",
                //TocTreePath = @"E:\Temp\AD_C3D_Docs\api\toctree.json",
                //PagesDir = @"E:\Temp\AD_C3D_Docs\api\output"

                WorkMode = "CreateHelp",
                TocTreePath = @"E:\Temp\AD_C3D_Docs\api\toctree.json",
                PagesDir = @"E:\Temp\AD_C3D_Docs\api\output",
                ServerPaths = "https://help.autodesk.com/cloudhelp/2025/ENU/Civil3D-API/files/html/,https://help.autodesk.com/cloudhelp/2025/ENU/"
            };
            RunOptions(options_test);
#endif

            Console.WriteLine("\nEnd!");
        }

        static void RunOptions(AD_HelpGen_Config opts)
        {
            AD_HelpGen_Config_Processing pr = new AD_HelpGen_Config_Processing(opts);
            pr.Run();
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Errors!");
            foreach (Error error in errs)
            {
                Console.WriteLine(error.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Text.Json;
using CommandLine;

namespace AD_HelpGen
{
    
    enum WorkModeVariant
    {
        GetLinks,
        ValidateLinks,
        CreateHelp
    }
    public class AD_HelpGen_Config
    {
        [Option(Required = true, HelpText = "The mode of working. " +
            "If == GetLinks: set TocTreePath and if need (LinksTextFile, BaseLink);" +
            "If == ValidateLinks: set TocTreePath and PagesDir;" + 
            "if == CreateHelp: set TocTreePath, PagesDir and if need (ServerPaths);")]
        public string? WorkMode { get; set; }

        [Option(Required = true, HelpText = "The path to toctree.json")]
        public string? TocTreePath { get; set; }



        [Option(Required = false, HelpText = "The name of target help, default help")]
        public string? HelpCaption { get; set; } = "help";

        [Option(Required = false, HelpText = "The part of path to files (used in raw HTML. If more use comma without tabs")]
        public string? ServerPaths { get; set; }



        [Option(Required = false, HelpText = "The path directory with downloaded pages")]
        public string? PagesDir { get; set; }

        [Option(Required = false, HelpText = "Text file name for links (from toctree.json), default _Links.txt")]
        public string? LinksTextFile { get; set; } = LinksTextFile_Default;

        [Option(Required = false, HelpText = "Address to help server without las slash, default https://help.autodesk.com")]
        public string? BaseLink { get; set; } = BaseLink_Default;

        public const string BaseLink_Default = "https://help.autodesk.com";

        public const string LinksTextFile_Default = "_Links.txt";

    }
}

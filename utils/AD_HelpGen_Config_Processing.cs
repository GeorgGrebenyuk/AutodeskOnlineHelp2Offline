using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace AD_HelpGen
{
    internal class TocTreeJson_Item
    {
        public string? ttl { get; set; }
        public string? ln { get; set; }
        public string? id { get; set; }

        public TocTreeJson_Item[]? children { get; set; }
    }
    internal class TocTreeJson
    {
        public TocTreeJson_Item[]? books { get; set; }

        public static TocTreeJson? LoadFrom(string path)
        {
            string json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<TocTreeJson>(json);
        }
    }
    internal class AD_HelpGen_Config_Processing
    {
        private readonly AD_HelpGen_Config? mConfig;

        public AD_HelpGen_Config_Processing(AD_HelpGen_Config? config)
        {
            this.mConfig = config;
            
        }

        public void Run()
        {
            if (this.mConfig == null) throw new Exception("mConfig is not defined");

            if (this.mConfig?.TocTreePath == null) throw new Exception("TocTreePath not specify!");
            if (!File.Exists(this.mConfig.TocTreePath)) throw new Exception("TocTreePath -- File not exists!");
            string saveDir = new FileInfo(this.mConfig.TocTreePath)?.DirectoryName ?? "";

            if (this.mConfig?.WorkMode == WorkModeVariant.GetLinks.ToString())
            {
                
                string savePath = Path.Combine(saveDir, this.mConfig.LinksTextFile ?? AD_HelpGen_Config.LinksTextFile_Default);
                File.WriteAllLines(savePath, GetLinks());
            }
            else if (this.mConfig?.WorkMode == WorkModeVariant.ValidateLinks.ToString())
            {
                if (this.mConfig?.PagesDir == null || !Directory.Exists(this.mConfig?.PagesDir)) throw new Exception("PagesDir it not valid path for directory -- it's not exists!");
                var links = GetLinks();

                var files = Directory.GetFiles(this.mConfig?.PagesDir, "*.*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f));

                foreach (var link in links)
                {
                    string link_file = link.Substring(link.LastIndexOf("/") + 1);
                    if (!files.Contains(link_file))
                    {
                        Console.WriteLine(link_file);
                    }
                }
            }
            else if (this.mConfig?.WorkMode == WorkModeVariant.CreateHelp.ToString())
            {
                if (this.mConfig?.PagesDir == null || !Directory.Exists(this.mConfig?.PagesDir)) throw new Exception("PagesDir it not valid path for directory -- it's not exists!");

                var links = GetLinks();
                var links_local = links.Select(link => link.Substring(link.LastIndexOf("/") + 1)).ToList();

                // Edit all internal links to local files inspite present it or not (TODO: comaring with "links"-variable data)
                if (mConfig.ServerPaths != null)
                {
                    string[] editServerPaths = new string[] { mConfig.ServerPaths };
                    if (mConfig.ServerPaths.Contains(",")) editServerPaths = mConfig.ServerPaths.Split(',');

                    foreach (string srcFile in Directory.GetFiles(this.mConfig?.PagesDir, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        string[] fileContent = File.ReadAllLines(srcFile);
                        for (int li = 0; li < fileContent.Length; li++)
                        {
                            string editedLine = fileContent[li];
                            foreach (string line in editServerPaths)
                            {
                                editedLine = editedLine.Replace(line, "");
                            }
                            fileContent[li] = editedLine;
                        }
                        File.WriteAllLines(srcFile, fileContent);
                    }
                }

                QT_QHP_File qtIndex = new QT_QHP_File(this.mConfig.HelpCaption);
                var TocTreeJsonFile = TocTreeJson.LoadFrom(this.mConfig.TocTreePath);

                foreach (TocTreeJson_Item? book in TocTreeJsonFile?.books ?? new TocTreeJson_Item[] { })
                {
                    child_processing(book, "");
                }

                void child_processing(TocTreeJson_Item? item, string ParentName)
                {
                    string pageName = getPageName(item.ln);
                    if (pageName != "")
                    {
                        qtIndex.AddPage(pageName, ParentName, item.ttl);
                    }
                    foreach (var ch in item?.children ?? new TocTreeJson_Item[] { })
                    {
                        child_processing(ch, pageName);                        
                    }
                }

                string getPageName(string ln)
                {
                    if (!ln.Contains("/")) return "";
                    return ln.Substring(ln.LastIndexOf("/") + 1);
                }

                string qtSavePath = Path.Combine(mConfig.PagesDir, "helpGen.qhp");
                qtIndex.Save(qtSavePath);
            }
        }

        private List<string> GetLinks()
        {
            List<string> links = new List<string>();
            var TocTreeJsonFile = TocTreeJson.LoadFrom(this.mConfig.TocTreePath);

            foreach (TocTreeJson_Item? book in TocTreeJsonFile?.books ?? new TocTreeJson_Item[] { })
            {
                if (book.ln != "") links.Add(MakeLink(book));
                child_processing(book);
            }

            void child_processing(TocTreeJson_Item? item)
            {
                foreach (var ch in item?.children ?? new TocTreeJson_Item[] { })
                {
                    if (ch.ln != "") links.Add(MakeLink(ch));
                    child_processing(ch);
                }
            }

            string MakeLink(TocTreeJson_Item? item) => $"{this.mConfig.BaseLink ?? AD_HelpGen_Config.BaseLink_Default}{item?.ln}";

            return links.Distinct().ToList();
        }



    }
}

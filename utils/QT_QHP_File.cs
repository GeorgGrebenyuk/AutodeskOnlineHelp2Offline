using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AD_HelpGen
{
    internal class QT_QHP_File
    {
        public string _namespace { get; set; }
        public string virtualFolder { get; set; }
        private XElement _toc_elements;
        private XElement _keywords_elements;
        private XElement _files_elements;


        public QT_QHP_File(string doc_namespace)
        {
            this._namespace = doc_namespace;
            this.virtualFolder = "html";
            _toc_elements = new XElement("toc");
            _files_elements = new XElement("files");
            _keywords_elements = new XElement("keywords");


        }

        private bool IsExists(string category, string value)
        {
            XElement searched_place;
            if (category == "keywords") searched_place = _keywords_elements;
            else if (category == "files") searched_place = _files_elements;
            else return false;

            bool is_exists = false;
            foreach (XElement elem_sub in searched_place.Elements())
            {
                if ((category == "keywords" && elem_sub.Attribute("name").Value == value) | (category == "files" && elem_sub.Value == value))
                {
                    is_exists = true;
                    break;
                }
            }
            return is_exists;
        }

        public void AddPage(string PageName, string ParentPageName, string title)
        {
            AddResource(PageName);
            XElement section_Element = new XElement("section",
                new XAttribute("title", title),
                new XAttribute("ref", PageName));

            bool stop = false;
            if (ParentPageName == "") _toc_elements.Add(section_Element);
            else ExploreNested(_toc_elements);



            void ExploreNested(XElement elem)
            {
                if (stop) return;
                foreach (XElement elem_sub in elem.Elements())
                {
                    if (stop) break;
                    if (elem_sub.Attribute("ref").Value == ParentPageName)
                    {
                        elem_sub.Add(section_Element);
                        stop = true;
                    }

                    if (elem_sub.Elements().Any()) ExploreNested(elem_sub);
                }
            }
        }

        public void AddResource(string fileNameWithExtension)
        {
            XElement file_Element = new XElement("file", fileNameWithExtension);
            if (!IsExists("files", fileNameWithExtension)) _files_elements.Add(file_Element);

        }

        public void AddKeyword(string name, string ref_path)
        {
            XElement keyword_Element = new XElement("keyword",
                new XAttribute("name", name),
                new XAttribute("ref", ref_path));
            if (!IsExists("keywords", name)) _keywords_elements.Add(keyword_Element);
        }

        public void Save(string path)
        {
            XDocument qhp_doc = new XDocument();
            XElement QtHelpProject_Element = new XElement("QtHelpProject", new XAttribute("version", "1.0"));
            QtHelpProject_Element.Add(new XElement("namespace", _namespace));
            QtHelpProject_Element.Add(new XElement("virtualFolder", virtualFolder));

            XElement filterSection_Element = new XElement("filterSection");
            filterSection_Element.Add(_toc_elements);
            filterSection_Element.Add(_keywords_elements);
            filterSection_Element.Add(_files_elements);
            QtHelpProject_Element.Add(filterSection_Element);

            qhp_doc.Add(QtHelpProject_Element);
            qhp_doc.Save(path);
        }
    }
}

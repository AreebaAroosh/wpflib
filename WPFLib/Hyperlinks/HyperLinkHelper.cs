using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO;
using Sgml;
using System.Xml;
using System.Web;
using System.Net;

namespace WPFLib.Hyperlinks
{
    public class HyperLinkHelper
    {
        public static string ReadText(string hyperlinkText)
        {
            var sb = new StringBuilder();

            hyperlinkText = "<html>" + hyperlinkText + "</html>";

            Sgml.SgmlReader rdr = new Sgml.SgmlReader();
            rdr.DocType = "HTML";
            rdr.WhitespaceHandling = WhitespaceHandling.All;
            rdr.CaseFolding = Sgml.CaseFolding.ToLower;
            rdr.InputStream = new StringReader(hyperlinkText);

            rdr.Read();
            while (!rdr.EOF)
            {
                if (rdr.IsStartElement() && rdr.Name == "a")
                {
                    if (rdr.HasAttributes)
                    {
                        while (rdr.MoveToNextAttribute())
                        {
                        }
                    }
                    rdr.ReadStartElement();

                    var content = rdr.ReadContentAsString();
					content = WebUtility.HtmlDecode(content);
                    rdr.ReadEndElement();
                    sb.Append(content);
                }
                else if (rdr.NodeType == System.Xml.XmlNodeType.Text)
                {
					sb.Append(WebUtility.HtmlDecode(rdr.Value));
                    rdr.Read();
                }
                else
                {
                    rdr.Read();
                }
            }

            return sb.ToString();
        }

        public static IEnumerable<Inline> Convert(string hyperlinkText)
        {
            hyperlinkText = "<html>" + hyperlinkText + "</html>";

            Sgml.SgmlReader rdr = new Sgml.SgmlReader();
            rdr.DocType = "HTML";
            rdr.WhitespaceHandling = WhitespaceHandling.All;
            rdr.CaseFolding = Sgml.CaseFolding.ToLower;
            rdr.InputStream = new StringReader(hyperlinkText);

            rdr.Read();
            while (!rdr.EOF)
            {
                if (rdr.IsStartElement() && rdr.Name == "a")
                {
                    var link = new Hyperlink();
                    if (rdr.HasAttributes)
                    {
                        while (rdr.MoveToNextAttribute())
                        {
                            if (rdr.Name == "href")
                            {
                                try
                                {
                                    var uriBild = new UriBuilder("shart", "service");
									uriBild.Path = Uri.UnescapeDataString(rdr.Value);
                                    link.NavigateUri = uriBild.Uri;
                                }
                                catch
                                {
                                    link.NavigateUri = null;
                                }
                            }
                        }
                    }
                    rdr.ReadStartElement();

                    var content = rdr.ReadContentAsString();
					content = WebUtility.HtmlDecode(content);
                	link.ToolTip = content;
                    rdr.ReadEndElement();
                    link.Inlines.Add(new Run(content));
                    yield return link;
                }
                else if (rdr.NodeType == System.Xml.XmlNodeType.Text)
                {
					var run = new Run(WebUtility.HtmlDecode(rdr.Value));
                    rdr.Read();
                    yield return run;
                }
                else
                {
                    rdr.Read();
                }
            }
        }

        public static string GetLinkString(string text, string link)
        {
            string format = "<a href=\"{0}\">{1}</a>";
			return String.Format(format, Uri.EscapeUriString(link), WebUtility.HtmlEncode(text));
        }
    }
}

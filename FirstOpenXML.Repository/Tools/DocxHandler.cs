using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FirstOpenXML.Repository.Tools
{
    public abstract class DocxHandler
    {
        public static void getDocxFromHtml(string html)
        {
            XElement xhtml = HtmlToWmlReadAsXElement.ReadAsXElement(html);

            string usedAuthorCss = @"";

            HtmlToWmlConverterSettings settings = HtmlToWmlConverter.GetDefaultSettings();
            //settings.BaseUriForImages = @"D:\";

            WmlDocument doc = HtmlToWmlConverter.ConvertHtmlToWml(
                defaultCss,
                usedAuthorCss,
                userCss,
                html,
                settings,
                null,
                annotatedHtml
            );
        }

        public class HtmlToWmlReadAsXElement
        {
            public static XElement ReadAsXElement(string htmlString)
            {
                htmlString = processHtml(htmlString);

                XElement html;
                try
                {
                    html = XElement.Parse(htmlString);
                }
                catch (XmlException)
                {
                    throw;
                }
                // HtmlToWmlConverter expects the HTML elements to be in no namespace, so convert all elements to no namespace.
                //html = (XElement)ConvertToNoNamespace(html);
                return html;
            }

            private static object ConvertToNoNamespace(XNode node)
            {
                XElement element = node as XElement;
                if (element != null)
                {
                    return new XElement(element.Name.LocalName,
                        element.Attributes().Where(a => !a.IsNamespaceDeclaration),
                        element.Nodes().Select(n => ConvertToNoNamespace(n)));
                }
                return node;
            }
        }

        private static string processHtml(string html)
        {
            string finalRS = string.Empty;
            var paragraphs = html.Split(@"</p>");

            foreach (var p in paragraphs)
            {
                var input = Regex.Replace(p, @"<p>", string.Empty);
                if (!input.Equals(string.Empty)) finalRS += $"<p>{tagsHandler(input)}</p>";
            }

            return $"<html><body>{finalRS}</body></html>";
        }

        private static string tagsHandler(string paragraph)
        {
            string startingTagPattern = @"<em>|<strong>|<u>";
            string endingTagPattern = @"</em>|</strong>|</u>";
            string tagPattern = @"(<(?:(?!\bspan\b)[^>])+>){1,3}|(</(?:(?!\bspan\b)[^>])+>){1,3}";

            bool first = true;
            string biu = string.Empty;
            string tempStyle = "font-size:12pt;";

            if (!paragraph[0].Equals('<'))
            {
                Console.WriteLine("no style at beginning! starting char: " + paragraph[0]);
                first = false;
                paragraph = @$"<span style=""{tempStyle}"">" + paragraph;
                biu += "n";
            }

            while (Regex.IsMatch(paragraph, tagPattern))
            {
                tempStyle = "font-size:12pt;";
                var match = Regex.Match(paragraph, tagPattern);

                if (Regex.IsMatch(match.Value, endingTagPattern))
                {
                    Console.WriteLine("ending: " + match.Value);
                    if (match.Value.Contains("</strong>"))
                    {
                        biu = biu.Replace("b", string.Empty);
                    }
                    if (match.Value.Contains("</em>"))
                    {
                        biu = biu.Replace("i", string.Empty);
                    }
                    if (match.Value.Contains("</u>"))
                    {
                        biu = biu.Replace("u", string.Empty);
                    }
                    paragraph = paragraph.Remove(match.Index, match.Value.Length);

                    if (paragraph[match.Index..].Length > 0)
                    {
                        tempStyle += getCss(biu);
                        tempStyle = @$"</span><span style=""{tempStyle}"">";
                        if (biu.Equals(string.Empty)) biu += "n";
                    }
                    else
                    {
                        tempStyle = @$"</span>";
                    }

                    paragraph = paragraph.Insert(match.Index, tempStyle);
                }
                else if (Regex.IsMatch(match.Value, startingTagPattern))
                {
                    Console.WriteLine("starting: " + match.Value);
                    biu = biu.Replace("n", string.Empty);
                    if (match.Value.Contains("<strong>"))
                    {
                        biu += "b";
                    }
                    if (match.Value.Contains("<em>"))
                    {
                        biu += "i";
                    }
                    if (match.Value.Contains("<u>"))
                    {
                        biu += "u";
                    }

                    tempStyle += getCss(biu);
                    tempStyle = (first) ? $@"<span style=""{tempStyle}"">" : @$"</span><span style=""{tempStyle}"">";
                    first = false;

                    paragraph = paragraph.Remove(match.Index, match.Value.Length).Insert(match.Index, tempStyle);
                }
            }

            if (biu.Contains("n")) paragraph += "</span>";

            Console.WriteLine(paragraph);
            return paragraph;
        }

        private static string getCss(string biu)
        {
            Dictionary<char, string> styles = new();
            styles.Add('b', "font-weight:bold;");
            styles.Add('i', "font-style:italic;");
            styles.Add('u', "text-decoration:underline;");
            styles.Add('n', "");

            string style = string.Empty;

            foreach (var c in biu)
            {
                style += styles[c];
            }

            return style;
        }


        #region static feilds
        private static readonly string defaultCss =
            @"html, address,
blockquote,
body, dd, div,
dl, dt, fieldset, form,
frame, frameset,
h1, h2, h3, h4,
h5, h6, noframes,
ol, p, ul, center,
dir, hr, menu, pre { display: block; unicode-bidi: embed }
li { display: list-item }
head { display: none }
table { display: table }
tr { display: table-row }
thead { display: table-header-group }
tbody { display: table-row-group }
tfoot { display: table-footer-group }
col { display: table-column }
colgroup { display: table-column-group }
td, th { display: table-cell }
caption { display: table-caption }
th { font-weight: bolder; text-align: center }
caption { text-align: center }
body { margin: auto; }
h1 { font-size: 2em; margin: auto; }
h2 { font-size: 1.5em; margin: auto; }
h3 { font-size: 1.17em; margin: auto; }
h4, p,
blockquote, ul,
fieldset, form,
ol, dl, dir,
menu { margin: auto }
a { color: blue; }
h5 { font-size: .83em; margin: auto }
h6 { font-size: .75em; margin: auto }
h1, h2, h3, h4,
h5, h6, b,
strong { font-weight: bolder }
blockquote { margin-left: 40px; margin-right: 40px }
i, cite, em,
var, address { font-style: italic }
pre, tt, code,
kbd, samp { font-family: monospace }
pre { white-space: pre }
button, textarea,
input, select { display: inline-block }
big { font-size: 1.17em }
small, sub, sup { font-size: .83em }
sub { vertical-align: sub }
sup { vertical-align: super }
table { border-spacing: 2px; }
thead, tbody,
tfoot { vertical-align: middle }
td, th, tr { vertical-align: inherit }
s, strike, del { text-decoration: line-through }
hr { border: 1px inset }
ol, ul, dir,
menu, dd { margin-left: 40px }
ol { list-style-type: decimal }
ol ul, ul ol,
ul ul, ol ol { margin-top: 0; margin-bottom: 0 }
u, ins { text-decoration: underline }
br:before { content: ""\A""; white-space: pre-line }
center { text-align: center }
:link, :visited { text-decoration: underline }
:focus { outline: thin dotted invert }
/* Begin bidirectionality settings (do not change) */
BDO[DIR=""ltr""] { direction: ltr; unicode-bidi: bidi-override }
BDO[DIR=""rtl""] { direction: rtl; unicode-bidi: bidi-override }
*[DIR=""ltr""] { direction: ltr; unicode-bidi: embed }
*[DIR=""rtl""] { direction: rtl; unicode-bidi: embed }
";

        private  static readonly string userCss = @"";
        #endregion
    }
}

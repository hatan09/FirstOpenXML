using System;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenXmlPowerTools;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FirstOpenXML.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HTMLController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetHtmlFromDocx(IFormFile file)
        {
            var fi = file.FileName;
            //byte[] byteArray = System.IO.File.ReadAllBytes(@"C:\Folders\TestDocs.docx");
            //var filePath = @"C:\Folders\TestDocs.docx";
            var tempPath = Path.GetTempFileName();

            using (FileStream stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose))
            {
                await file.CopyToAsync(stream);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, true))
                {
                    var pageTitle = fi.Split(".")[0];
                    
                    WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                    {
                        AdditionalCss = "body { margin: 1cm auto; padding: 0; font-family:arial; font-size: 20px}",
                        PageTitle = pageTitle,
                        //FabricateCssClasses = true,
                        RestrictToSupportedNumberingFormats = false
                    };
                    XElement xhtml = WmlToHtmlConverter.ConvertToHtml(wordDocument, settings);

                    var html = new XDocument(
                        new XDocumentType("html", null, null, null),
                        xhtml);

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);
                    var firstBody = htmlString.IndexOf("<div>");
                    var css = htmlString.Substring(0, firstBody);
                    var classes = cssHandler(css);
                    htmlString = htmlString.Remove(0, firstBody + 5);
                    var lastBody = htmlString.LastIndexOf("</div>");
                    htmlString = htmlString.Remove(lastBody);
                    System.IO.File.WriteAllText(@"D:\text.html", htmlString, Encoding.UTF8);
                    string cssClass = string.Empty;
                    for(int i = 0; i < classes.Length; i++)
                    {
                        cssClass = cssClass + "\n nhattan \n" + classes[i];
                    }
                    System.IO.File.WriteAllText(@"D:\css.html", cssClass, Encoding.UTF8);
                }
            }

            //var rsStream = System.IO.File.Open(filePath, FileMode.OpenOrCreate);

            return Ok();
        }

        string[] cssHandler(string cssString)
        {
            cssString = cssString.Remove(0, cssString.IndexOf("<style>") + "<style>".Length);
            cssString = cssString.Remove(cssString.LastIndexOf("</style>"));

            var classes = cssString.Split("}");
            foreach(var c in classes)
            {
                var parts = c.Split("{", 2);
                var className = parts[0].Split("pt-", 2)[1].Trim();
                var style = styleHandler(parts[1]);
            }
            return classes;
        }

        string styleHandler(string css)
        {
            var elements = css.Split("\n");
            foreach(var e in elements)
            {
                if (e.Contains("font-weight"))
                {
                    getStyle(e);
                }
            }

            return string.Empty;
        }

        char getStyle(string element)
        {
            var styleName = element.Split(":", 2)[1].Trim().Replace(";", string.Empty);
            
            return (styleName.Equals("bold") || styleName.Equals("italic") || styleName.Equals("underline")) ? styleName[0] : 'n';
        }
    }
}

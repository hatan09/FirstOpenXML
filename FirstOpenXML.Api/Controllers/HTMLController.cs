using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenXmlPowerTools;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using FirstOpenXML.Repository.Tools;
using FirstOpenXML.Contracts;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using FirstOpenXML.Api.DataObjects;
using System.Xml;

namespace FirstOpenXML.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HTMLController : ControllerBase
    {
        //private readonly IMapper _mapper;
        private readonly IHtmlRepository _htmlRepository;

        public HTMLController(IHtmlRepository htmlRepository)
        {
            //_mapper = mapper;
            _htmlRepository = htmlRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHtml(CancellationToken cancellationToken = default)
        {
            var htmls = await _htmlRepository.FindAll().ToListAsync(cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetDocxByHtmlId(int id, CancellationToken cancellationToken = default)
        {
            var htmlFile = await _htmlRepository.FindByIdAsync(id, cancellationToken);
            if (htmlFile is null) return BadRequest($"No file with id {id}");

            XElement x = HtmlHandler.processHtmlForConvertToDocx(htmlFile.Content);

            string usedAuthorCss = @"";

            HtmlToWmlConverterSettings settings = HtmlToWmlConverter.GetDefaultSettings();
            //settings.BaseUriForImages = @"D:\";

            WmlDocument doc = HtmlToWmlConverter.ConvertHtmlToWml(
                "",
                usedAuthorCss,
                "",
                x,
                settings,
                null,
                null
            );

            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms))
            {
                
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetHtmlFromDocx(IFormFile file)
        {
            string rs = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, true))
                {
                    WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                    {
                        FabricateCssClasses = false,
                    };

                    XElement xhtml = WmlToHtmlConverter.ConvertToHtml(wordDocument, settings);

                    var html = new XDocument(
                        new XDocumentType("html", null, null, null),
                        xhtml);

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);

                    rs = HtmlHandler.processHtmlForQuill(htmlString);
                }
            }

            return Ok(new {html = rs});
        }

        [HttpPost]
        public async Task<IActionResult> UploadHtml(HTMLFileDTO dto)
        {
            //_htmlRepository.Add(dto);
            return Ok();
        }
    }
}

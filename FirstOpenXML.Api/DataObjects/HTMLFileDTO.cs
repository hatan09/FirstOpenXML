using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstOpenXML.Api.DataObjects
{
    public class HTMLFileDTO : BaseEntityDTO
    {
        public string? Title { get; set; } = string.Empty;
        public int? FontSize { get; set; } = 12;
        public string FontStyle { get; set; } = "Times New Roman";
        public string? Content { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
    }
}

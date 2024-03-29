﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstOpenXML.Core.Entities
{
    public class HTMLFile : BaseEntity
    {
        public string? Title { get; set; } = string.Empty;
        public int? FontSize { get; set; } = 12;
        public string FontStyle { get; set; } = "Times New Roman";
        public string? Content { get; set; } = string.Empty;
        public User? User { get; set; } = null;
        public string? UserId { get; set; } = string.Empty;

    }
}

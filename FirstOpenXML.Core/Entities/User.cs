using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstOpenXML.Core.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; } = string.Empty;

        public virtual ICollection<HTMLFile> HTMLFiles { get; set; } = new HashSet<HTMLFile>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}

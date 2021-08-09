using FirstOpenXML.Contracts;
using FirstOpenXML.Core.Database;
using FirstOpenXML.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstOpenXML.Repository
{
    public class HtmlRepository : RepositoryBase<HTMLFile>, IHtmlRepository
    {
        public HtmlRepository(AppDbContext adbc) : base(adbc) { }
        public void GetHtmlFromDocx(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}

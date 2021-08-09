using FirstOpenXML.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstOpenXML.Contracts
{
    public interface IHtmlRepository : IRepositoryBase<HTMLFile>
    {
        public void GetHtmlFromDocx(FileStream stream);
    }
}

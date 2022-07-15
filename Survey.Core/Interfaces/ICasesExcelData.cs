using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ICasesExcelData : IRepository<CasesExcelData>
    {
        List<CasesExcelData> GetFileData(int obj);
    }
}

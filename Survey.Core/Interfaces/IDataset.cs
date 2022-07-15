using Survey.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IDataset : IRepository<Dataset>
    {
        public IEnumerable<SelectListItem> GetAllDatasets();
        void Update(Dataset dataset);
    }
}

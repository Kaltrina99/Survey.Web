using Survey.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IProjects : IRepository<Projects>
    {
        void Update(Projects objProject);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
        List<Projects> GetProjects(int? filterpojectid, string searchkeyword);
    }
}

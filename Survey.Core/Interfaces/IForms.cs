using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using Survey.Core.Filter;

namespace Survey.Core.Interfaces
{
    public interface IForms : IRepository<Forms>
    {
        void Update(Forms forms);
        Forms GetForm(int id);
        bool UpdateForm(Forms form);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
        Task<ServiceResponse<XSSFWorkbook>> CreateExcelFromForm(int id);
        Task<ServiceResponse<bool>> CreateFormFromExcel(IFormFile file,string tenantid,int id);
        IPagedList<Forms> GetForms(List<int> clientId, List<int> projectId, FormFilter filter,int pagenumber,int pagesize);
        List<Projects> GetProjects(List<int> clientId, List<int> projectId);

    }
}

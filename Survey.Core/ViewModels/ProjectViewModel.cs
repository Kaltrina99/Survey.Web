using Survey.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class ProjectViewModel
    {
        public Projects Project { get; set; }
        public ProjectCategory ProjectCategory { get; set; }
        public List<Projects> Projects { get; set; }
        public List<ProjectCategory> GetClients { get; set; }
        public IEnumerable<ProjectCategory> ProjectCategories { get; set; }
        public IEnumerable<SelectListItem> ProjectCategorySelectList { get; set; }
        public IEnumerable<SelectListItem> ProjectSelectList { get; set; }
        public IEnumerable<SelectListItem> ProjectAssignmentList { get; set; }
    }
}

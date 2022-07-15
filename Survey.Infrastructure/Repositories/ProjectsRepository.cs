using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class ProjectsRepository : Repository<Projects>, IProjects
    {
        private readonly ApplicationDbContext _dbContext;

        public ProjectsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Projects objProject)
        {
            _dbContext.Projects.Update(objProject);
        }

        
        public List<Projects> GetProjects(int? filterclientid, string searchingword)
        {
            List<Projects> results = new List<Projects>();
            var qury = _dbContext.Projects.ToList();
            
             var projectlist = qury;
            return projectlist;

        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WebConstants.Category)
            {
                return _dbContext.ProjectCategories.Select(pc => new SelectListItem
                {
                    Value = pc.Id.ToString(),
                    Text = pc.Name
                });
            }
            if (obj == WebConstants.Project)
            {
                return _dbContext.Projects.Select(proj => new SelectListItem
                {
                    Value = proj.Id.ToString(),
                    Text = proj.Title
                });
            }

            return null;
        }
    }
}

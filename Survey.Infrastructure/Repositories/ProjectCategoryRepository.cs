using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class ProjectCategoryRepository : Repository<ProjectCategory>, IProjectCategory
    {
        private readonly ApplicationDbContext _dbContext;
        public ProjectCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProjectCategory> GetClients()
        {
            List<ProjectCategory> results = new List<ProjectCategory>();

            
            return results;
        }

        public void Update(ProjectCategory objProjectCategory)
        {
            var objFromDb = _dbContext.ProjectCategories.FirstOrDefault(pc => pc.Id == objProjectCategory.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = objProjectCategory.Name;
            }
        }
    }
}

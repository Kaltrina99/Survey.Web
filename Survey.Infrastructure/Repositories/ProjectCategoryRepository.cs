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

        public List<ProjectCategory> GetClients(List<int> clientId, List<int> projectId)
        {
            List<ProjectCategory> results = new List<ProjectCategory>();

            foreach (var id in clientId)
            {
                var clients = _dbContext.ProjectCategories.Where(x => x.Id == id).ToList();
                foreach (var client in clients)
                {
                    results.Add(client);
                }
            }
            foreach (var id in projectId)
            {
                var projects = _dbContext.Projects.Where(x => x.Id == id).ToList();
                foreach (var project in projects)
                {
                    var client = _dbContext.ProjectCategories.Where(x => x.Id == project.ProjectCategoryId).FirstOrDefault();

                    bool alreadyExists = results.Contains(client);
                    if (!alreadyExists)
                    { 
                         results.Add(client);
                    }
                }
            }
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

using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class SkipLogicRepository : Repository<SkipLogic>, ISkipLogic
    {
        private readonly ApplicationDbContext _dbContext;

        public SkipLogicRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetSkipLogic(int id)
        {
         return _dbContext.SkipLogic.Where(x => x.Child_Question_Id == id).Select(x => x.Parent_Question_Id).FirstOrDefault();
        }

        public IEnumerable<SkipLogic> GetSkipLogicById(int? id)
        {
            return _dbContext.SkipLogic.Where(x => x.Parent_Question_Id == id || x.Child_Question_Id == id);
        }

        public List<SkipLogic> GetSkipLogicByQuestionId(int id)
        {
            return _dbContext.SkipLogic.Where(x => x.Parent_Question_Id == id || x.Child_Question_Id == id).ToList();
        }

        public IEnumerable<SkipLogic> GetSkipLogicByQuestionwithparentId(int id)
        {
            return  _dbContext.SkipLogic.Include(x=>x.ParentQuestion).Include(x=>x.Option).Where(x => x.Child_Question_Id == id).ToList();

        }

        public int? GetSkipLogicCondition(int id)
        {
            return _dbContext.SkipLogic.Where(x => x.Child_Question_Id == id).Select(x => x.Condition_Option).FirstOrDefault();
        }
    }
}

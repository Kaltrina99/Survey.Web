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
    public class AnswersRepository : Repository<Answers>, IAnswers
    {
        private readonly ApplicationDbContext _dbContext;

        public AnswersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Answers objAnswer)
        {
            _dbContext.Answers.Update(objAnswer);
        }
        public IEnumerable<Answers> GetAnswersByFormId(int? obj)
        {
            return _dbContext.Answers.Where(an => an.Form_Id == obj);
        }
    }
}

using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class QuestionOptionsRepository : Repository<QuestionOptions>, IQuestionOptions
    {
        private readonly ApplicationDbContext _dbContext;

        public QuestionOptionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(QuestionOptions objOptions)
        {
            _dbContext.QuestionOptions.Update(objOptions);
        }
        public IEnumerable<MultiValueSelectItem> GetOptions()
        {
            return _dbContext.QuestionOptions.Select(op => new MultiValueSelectItem
            {
                Value1 = op.Id.ToString(),
                Value2 = op.Question_Id.ToString(),
                Text = op.OptionText
            });
        }

        public List<QuestionOptions> GetOptionsByQuestionId(int obj)
        {
            return _dbContext.QuestionOptions.Where(qo => qo.Question_Id == obj).ToList();
        }

        public FormViewModel GetConditionOptions(int obj)
        {
            var model = new FormViewModel();

            var question = _dbContext.Questions.FirstOrDefault(x => x.Id == obj);
            var operators = Operator.GetOperators(Convert.ToInt32(question.Field_Type));
            model.InputId = Convert.ToInt32(question.Field_Type);
            model.OperatorsList = operators;
            model.ConditionOption = _dbContext.QuestionOptions.Where(x => x.Question_Id == obj).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.OptionText
            });
            return model;
        }

        public QuestionOptions GetOptionById(int obj)
        {
            return _dbContext.QuestionOptions.Where(x => x.Id == obj).FirstOrDefault();
        }

        public IEnumerable<QuestionOptions> GetCondition(int obj)
        {
            return _dbContext.QuestionOptions.Where(x => x.Id == obj);
        }
    }
}

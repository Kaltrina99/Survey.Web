using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IQuestionOptions : IRepository<QuestionOptions>
    {
        void Update(QuestionOptions objOptions);
        IEnumerable<MultiValueSelectItem> GetOptions();
        List<QuestionOptions> GetOptionsByQuestionId(int obj);
        FormViewModel GetConditionOptions(int obj);
        public QuestionOptions GetOptionById(int obj);
        public IEnumerable<QuestionOptions> GetCondition(int obj);
    }
}

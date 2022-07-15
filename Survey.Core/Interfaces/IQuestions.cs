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
    public interface IQuestions : IRepository<Questions>
    {
        void Update(Questions objQuestion);
        Task<int> Count(int id);

        List<Questions> GetQuestions(int obj);
        IEnumerable<Questions> AllQuestions();
        IEnumerable<Questions> GetQuestionsByFormId(int? obj);
        int GetQuestionsFormId(int obj);
        IEnumerable<Questions> GetQuestionById(int obj);
        IEnumerable<SelectListItem> ParentQuestions(int obj, int obj2);
        public int GetQuestionType(int obj);
        public Questions GetOldQuestion(int id);
        public Task<TakeSurveyViewModel> StartSurvey(int id);

        public SkipLogicViewModel ShowSkippedQuestions(int id, int language);
        public SkipLogicViewModel GetSkippedQuestionNumber(int id, int language);
        ServiceResponse<SkipLogicViewModel> DeleteQuestion(int id);
        ServiceResponse<FormViewModel> getQuestion(int id);
        ServiceResponse<bool> UpdateQuestion(Questions question,string[] newoptions);
        Task<ServiceResponse<bool>> UpdateOrder(List<Questions> questions,int id);
    }
}

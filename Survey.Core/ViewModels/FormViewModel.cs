using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using X.PagedList;
using Survey.Core.Filter;
using Microsoft.AspNetCore.Identity;

namespace Survey.Core.ViewModels
{
    public class FormViewModel
    {
        public FormFilter Filter { get; set; }
        public List<IFormFile> File { get; set; } = new List<IFormFile>();
        public string[] otherOptions { get; set; }
        public int formid { get; set; }
        public int ProjectId { get; set; }
        public Forms Form { get; set; }
        public Questions Question { get; set; }
        public List<ProjectCategory> GetClients { get; set; }
        public string QuestionDescription { get; set; }
        public QuestionOptions Option { get; set; }
        public Answers Answer { get; set; }
        public List<Projects> GetProjects { get; set; }

        public SkipLogic SkipLogic { get; set; }
        public IEnumerable<Questions> OldQuestion { get; set; }
        public IPagedList<Forms> Forms { get; set; }
        public List<Projects> ProjectList { get; set; } = new List<Projects>();
        public IEnumerable<MultiValueSelectItem> QuestionsList { get; set; }
        public IEnumerable<SelectListItem> ParentQuestionsList { get; set; }
        public IEnumerable<SelectListItem> ConditionOption { get; set; }
        public List<Questions> OriginalQuestion { get; set; }
        public IEnumerable<QuestionOptions> Condition { get; set; }
        public List<QuestionOptions> OriginalOptions { get; set; }
        public List<SkipLogic> SkipLogicList { get; set; }
        public List<Questions> ParentTest { get; set; }
        public List<Operators> OperatorsList { get; set; }
        public int InputId { get; set; }
        public int index { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string SurveyLink { get; set; }
        public QType QuestionType { get; set; }
        public QuestionOptions option { get; set; }
        public IList<QuestionOptions> _Options { get; set; }
        public int OptionId { get; set; }
        public string answer { get; set; }
        public bool isParent { get; set; }
        public bool wasPublished { get; set; }

        public List<IdentityUser> Users { get; set; }
        public IFormFile Excel { get; set; }

    }
}

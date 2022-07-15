using Survey.Core.HelperClasses;
using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.DTOmodels
{
    public class DownloadJsonDto
    {
        private Forms Form { get; set; }
        public DtoForm FormDto { get; set; } = new DtoForm();
        public DownloadJsonDto(Forms form)
        {
            this.Form = form;
            GetDto();
        }
        public void GetDto()
        {
            FormDto.Title = Form.FormTitle;
            var questionlists = Form.Questions.ToList();
            foreach (var question in questionlists) 
            {
                DtoExpoerQuestion ques = new DtoExpoerQuestion();
                ques.QuestionText = question.QuestionText;
                ques.QuestionType = question.Field_Type.ToString();
                for (int i = 0; i < question.Options.Count(); i++)
                {
                    var index = i + 1;
                    DtoOption opt = new DtoOption();
                    opt.Option = question.Options[i].OptionText;
                    opt.OptionId = index;
                    ques.Options.Add(opt);
                }
                FormDto.Questions.Add(ques);
            }
            foreach (var submission in Form.Submissions) 
            {
                DtoSubmission sub = new();
                sub.EndDate = (DateTime)submission.EndTime;
                sub.StartDate = submission.StartTime;
                foreach (var question in questionlists)
                {
                    DtoExpoerQuestion ques = new DtoExpoerQuestion();
                    ques.QuestionText = question.QuestionText;
                    ques.QuestionType = question.Field_Type.ToString();
                    for (int i=0;i<question.Options.Count();i++)
                    {
                        var index = i + 1;
                        DtoOption opt = new DtoOption();
                        opt.Option = question.Options[i].OptionText;
                        opt.OptionId = index;
                        ques.Options.Add(opt);
                    }
                    foreach (var answer in question.Answers.Where(x=>x.SubmissionId==submission.Id)) 
                    {
                        DtoAnswers an = new();
                        an.Answer = answer.Answer;
                        if (answer.OptionId is not null) 
                        {
                           var optid = ques.Options.FirstOrDefault(x => string.Equals(x.Option,answer.Option.OptionText)).OptionId;
                           an.optionId = optid;
                        }
                        ques.Answers.Add(an);
                    }
                    sub.Questions.Add(ques);
                }
                FormDto.Submissions.Add(sub);
            }
        }
    }
    public class DtoForm
    {
        public string Title { get; set; }
        public List<DtoExpoerQuestion> Questions { get; set; } = new List<DtoExpoerQuestion>();
        public List<DtoSubmission> Submissions { get; set; } = new List<DtoSubmission>();
    }
    public class DtoSubmission
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DtoExpoerQuestion> Questions { get; set; } = new List<DtoExpoerQuestion>();

    }
    public class DtoAnswers
    {
        public string Answer { get; set; }
        public int? optionId { get; set; }
    }
    public class DtoExpoerQuestion
    {
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public List<DtoOption> Options { get; set; } = new List<DtoOption>();
        public List<DtoAnswers> Answers { get; set; } = new List<DtoAnswers>();
    }
    public class DtoOption
    {
        public int OptionId { get; set; }
        public string Option { get; set; }
    }
   
}

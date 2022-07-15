using Survey.Core.DTOmodels;
using Survey.Core.HelperClasses;
using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.HelperClasses
{
    public class FormReport
    {
        private readonly Forms _form;
        public int TotalFomrSubmissions { get; set; }
        public List<Questions> QuestionReports { get; set; } = new List<Questions>();
        public FormReport(Forms form)
        {
            _form = form;
            TotalFomrSubmissions = _form.Submissions.Count();
            SetResults();
        }
     
        private void SetResults() 
        {
            _form.Submissions.RemoveAll(x => x.EndTime == null);
            foreach (var question in _form.Questions) 
            {
               Questions report = GenerateResult(question);
                QuestionReports.Add(report);
            }
        }
        private Questions  GenerateResult(Questions question) 
        {
            if (question.Field_Type == QType.Select_One || question.Field_Type == QType.Select_Multiple)
            {
                question.TotalSubmissions = question.Answers.GroupBy(x => x.SubmissionId).Count();
                question.TotalSkiped = TotalFomrSubmissions - question.TotalSubmissions;
                foreach (var option in question.Options)
                {

                    option.TotalSelected = question.Answers.Where(x => x.OptionId == option.Id).Count();

                    option.Percetage = ((double)option.TotalSelected / (double)question.TotalSubmissions) * 100;
                    option.Percetage = Math.Round(option.Percetage, 2);
                }
            }
            else 
            {
                question.TotalSubmissions = question.Answers.GroupBy(x => x.SubmissionId).Count();
                question.TotalSkiped = TotalFomrSubmissions - question.TotalSubmissions;
            }
            return question;
        }
       
       

    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class Answers
    {
        [Key]
        public int Id { get; set; }
        public int? Form_Id { get; set; }
        public string? Team_Id { get; set; }
        public string? Tenant_Id { get; set; }
        public int Question_Id { get; set; }
        public int? OptionId { get; set; }
        public virtual  QuestionOptions Option { get; set; }
        [Display(Name = "Answer")]
        public string Answer { get; set; }
       
        [ForeignKey("Form_Id")]
        public virtual Forms form { get; set; }
        [Required]
        [ForeignKey("Question_Id")]
        public virtual Questions question { get; set; }
        public int? SubmissionId { get; set; }
        public virtual SurveySubmission Submission { get; set; }
       
       
    }
}

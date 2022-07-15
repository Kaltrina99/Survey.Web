using Survey.Core.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class Forms
    {
        public Forms()
        {
            Questions = new List<Questions>();
            Answers = new List<Answers>();
           /* CheckBoxAnswers = new List<CheckBoxAnswers>();*/
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        [Display(Name = "Title")]
        public string FormTitle { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage ="You must select a project")]
        public int? Project_Id { get; set; }
        [ForeignKey("Project_Id")]
        public virtual Projects Project { get; set; }
        public FormStatus Form_Status { get; set; }
        public bool WasPublished { get; set; }

        public List<Questions> Questions { get; set; }
        public IList<Answers> Answers { get; set; }
       /* public IList<CheckBoxAnswers> CheckBoxAnswers { get; set; }*/
        public virtual List<SurveySubmission> Submissions { get; set; }
        public virtual List<SurveyDownload> SurveyDownloads { get; set; }
      
    }
}

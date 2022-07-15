using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class QuestionOptions
    {
        public QuestionOptions()
        {
          /*  CheckBoxAnswers = new List<CheckBoxAnswers>();*/
            ChildOption = new List<SkipLogic>();
            isSelected = new bool();
        }
        [Key]
        public int Id { get; set; }
        public int Question_Id { get; set; }
        [Required]
      
        [Display(Name = "OptionText")]
        public string OptionText { get; set; }
        public bool isSelected { get; set; }
        [ForeignKey("Question_Id")]
        public Questions question { get; set; }
       /* public IList<CheckBoxAnswers> CheckBoxAnswers { get; set; }*/
        public List<SkipLogic> ChildOption { get; set; }
        [NotMapped]
        public bool Skip_Logic { get; set; }
        public int OrderNumber { get; set; }
        public string? Team_Id { get; set; }
        public string? Tenant_Id { get; set; }
        public virtual List<Answers> OptionAnswer { get; set; }
        #region report
        [NotMapped]
        public int TotalSelected { get; set; }
        [NotMapped]
        public double Percetage { get; set; }
        #endregion
    }
}

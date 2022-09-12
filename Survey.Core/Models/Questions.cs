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
    public class Questions
    {
        public Questions()
        {
            Options = new List<QuestionOptions>();
            Answers = new List<Answers>();
            skipChild = new List<SkipLogic>();
            skipParent = new List<SkipLogic>();
        }
        [Key]
        public int Id { get; set; }


        public QType Field_Type { get; set; }
        public int Form_Id { get; set; }
        [Display(Name = "Required")]
        public bool IsRequired { get; set; }
        [Required]
        [Display(Name = "QuestionText")]
        public string QuestionText { get; set; }
        public string QuestionDescription { get; set; }
        [NotMapped]
        public bool Skip_Logic { get; set; }
        public int QuestionOrder { get; set; }
        public SkipLogicType SkipLogicType { get; set; }
        [Required]
        [ForeignKey("Form_Id")]
        public virtual Forms form { get; set; }
        public virtual List<SkipLogic> skipChild { get; set; } = new List<SkipLogic>();
        public virtual List<SkipLogic> skipParent { get; set; } = new List<SkipLogic>();

        public List<QuestionOptions> Options { get; set; } = new List<QuestionOptions>();
        public IList<Answers> Answers { get; set; }

        #region report 
        [NotMapped]
        public int TotalSubmissions { get; set; }
        [NotMapped]
        public int TotalSkiped { get; set; }

        #endregion

    }
}

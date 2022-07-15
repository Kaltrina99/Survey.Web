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
    public class SkipLogic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Parent_Question_Id { get; set; }
        public int Child_Question_Id { get; set; }
        public int? Condition_Option { get; set; }
        public int Operator { get; set; }
        [NotMapped]
        public string operator_string { get; set; }
        public double? Comparable_Value { get; set; }
        public string? Team_Id { get; set; }
        public string? Tenant_Id { get; set; }

        [ForeignKey("Parent_Question_Id")]
        public virtual Questions ParentQuestion { get; set; }
        [ForeignKey("Child_Question_Id")]
        public virtual Questions ChildQuestion { get; set; }
        [ForeignKey("Condition_Option")]
        public virtual QuestionOptions Option { get; set; }

    }
}

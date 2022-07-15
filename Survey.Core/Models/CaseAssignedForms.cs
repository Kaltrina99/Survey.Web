using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class CaseAssignedForms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Dataset_Id { get; set; }
        public int Case_Id { get; set; }
        public int Form_Id { get; set; }
        public int CaseExcelData_Id { get; set; }
        public string Assigned_Form { get; set; }
    }
}

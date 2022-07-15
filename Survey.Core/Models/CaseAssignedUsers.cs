using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class CaseAssignedUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Dataset_Id { get; set; }
        public int Case_Id { get; set; }
        public int CaseExcelData_Id { get; set; }
        public string Assigned_To { get; set; }
        public string Assigned_to_Id { get; set; }
        public int Assigned_Type { get; set; }
    }
}

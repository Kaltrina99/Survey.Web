using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class CasesExcelHeaders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Dataset_Id { get; set; }
        public int Column_Id { get; set; }
        public string Header { get; set; }
        [Required]
        [ForeignKey("Dataset_Id")]
        public Dataset dataset { get; set; }
    }
}

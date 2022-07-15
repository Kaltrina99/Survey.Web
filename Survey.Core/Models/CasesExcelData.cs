using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class CasesExcelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Dataset_Id { get; set; }
        public int Case_Id { get; set; }
        public int Header_Id { get; set; }
        public string Cell_Text { get; set; }
        //[Required]
        //[ForeignKey("Case_Id")]
        //public Cases cases { get; set; }
        [Required]
        [ForeignKey("Dataset_Id")]
        public Dataset dataset { get; set; }

    }
}

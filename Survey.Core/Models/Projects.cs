using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class Projects
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [MaxLength(25)]
        [Display(Name = "Code")]
        public string Code { get; set; }   
        [Display(Name = "Project Category Type")]
        public int ProjectCategoryId { get; set; }
        [ForeignKey("ProjectCategoryId")]
        public virtual ProjectCategory ProjectCategory { get; set; }
    }
}

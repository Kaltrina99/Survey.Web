using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class ProjectCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        [Display(Name = "Title")]
        public string Name { get; set; }
    }
}

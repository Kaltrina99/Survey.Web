﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int? ParentID { get; set; }

        [NotMapped]
        public virtual List<UserProjectCategory> UserList { get; set; }
        [ForeignKey("ParentID")]
        public virtual ProjectCategory Parent { get; set; }
        public virtual ICollection<ProjectCategory> Childs { get; set; }
        
    }
}

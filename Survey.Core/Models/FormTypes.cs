using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class FormTypes
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(25)]
        public string Type { get; set; }
    }
}

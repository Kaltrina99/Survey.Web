using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class UsersViewModel
    {
        [Key]
        public string Id { get; set; }
        public string FullName { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        public bool EmailConfirmed = true;

        [Required]
        public string Password { get; set; }
    }
}

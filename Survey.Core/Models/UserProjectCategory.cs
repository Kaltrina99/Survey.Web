using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class UserProjectCategory
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CategoryId { get; set; }
        public ProjectCategory Category { get; set; }
    }
}

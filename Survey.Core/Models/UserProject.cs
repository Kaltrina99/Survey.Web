using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class UserProject
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProjectsId { get; set; }
        public Projects Projects { get; set; }
    }
}

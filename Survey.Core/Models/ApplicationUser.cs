using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> RoleList { get; set; }
        [NotMapped]
        public virtual List<UserProjectCategory> CategoryList { get; set; }
        [NotMapped]
        public virtual List<SurveyDownload> SurveyDownloads { get; set; }
        [NotMapped]
        public ICollection<SurveySubmission> SurveySubmissions { get; set; }
    }
}

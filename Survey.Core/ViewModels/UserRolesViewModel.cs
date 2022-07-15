using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public List<UserRolesViewModel> UserRoles { get; set; } = new List<UserRolesViewModel>();
    }

    public class UserRolesViewModel
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}

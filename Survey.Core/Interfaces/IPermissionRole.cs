using Survey.Core.HelperClasses;
using Survey.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IPermissionRole
    {
        Task<ServiceResponse<List<RoleClaimsViewModel>>> GetAllPermissionsForRole(string roleId);
        Task<ServiceResponse<bool>> UpdatePermissionForRole(List<string> claims,string roleId);
    }
}

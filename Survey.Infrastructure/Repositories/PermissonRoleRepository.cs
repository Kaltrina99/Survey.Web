using Survey.Core.Constants;
using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class PermissonRoleRepository : IPermissionRole
    {
        private readonly ApplicationDbContext _db;

        public PermissonRoleRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<ServiceResponse<List<RoleClaimsViewModel>>> GetAllPermissionsForRole(string roleId)
        {
            ServiceResponse<List<RoleClaimsViewModel>> response = new ServiceResponse<List<RoleClaimsViewModel>>();
            response.Data = new List<RoleClaimsViewModel>();
            try
            {
                var roleClaims=  await _db.RoleClaims.Where(x => x.RoleId == roleId)
                    .Select(x=> x.ClaimValue).ToListAsync();
                List<RoleClaimsViewModel> allClaimRoles = Permissions.PremissionList.GetAllUserPermissionList().Select(x=>new RoleClaimsViewModel { Value=x}).ToList();
                allClaimRoles.ForEach(x=> {
                    if (roleClaims.Contains(x.Value)) 
                    {
                        x.Selected = true;
                    }
                });
                if (roleClaims==null)
                {
                    response.Message = "No Claims Found";
                    return response;
                }
                response.Data = allClaimRoles;
            }
            catch(Exception e)
            {
                response.Success = false;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdatePermissionForRole(List<string> newclaims, string roleId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
          
            try
            {
                var role = await _db.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
                if (role == null)
                {
                    response.Success = false;
                    response.Message = "Role Not Found";
                    return response;
                }
                var ClaimList = await _db.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync();
                _db.RoleClaims.RemoveRange(ClaimList);
                await _db.SaveChangesAsync();
                List<IdentityRoleClaim<string>> newRoleClaims = newclaims.Select(x => new IdentityRoleClaim<string> {RoleId=roleId,ClaimValue=x,ClaimType= "Permission" }).ToList();
               await _db.RoleClaims.AddRangeAsync(newRoleClaims);
                await _db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                response.Success = false;
            }
            return response;
        }
    }
}

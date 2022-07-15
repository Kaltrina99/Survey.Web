using Survey.Core.Constants;
using Survey.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Survey.Core.Constants.Permissions;

namespace Survey.Infrastructure.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            
            var hasher = new PasswordHasher<IdentityUser>();

            #region IdentityUsersSeed
            IdentityUser superAdminAccount = new IdentityUser
            {
                Id = "1",
                UserName = "SuperAdmin",
                Email = "admin@Survey.com",
                NormalizedEmail = "admin@Survey.com".ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            superAdminAccount.PasswordHash = hasher.HashPassword(superAdminAccount, "P@ssw0rd");

            modelBuilder.Entity<IdentityUser>().HasData(superAdminAccount);
            #endregion

            #region RolesSee
            IdentityRole AdminRole = new IdentityRole()
            {
                Id="1",
                Name = Roles.Admin.ToString(),
                NormalizedName = Roles.Admin.ToString().ToUpper()
            };
            
            modelBuilder.Entity<IdentityRole>().HasData(AdminRole);
            #endregion

            #region IdentityUserRole
            
            IdentityUserRole<string> SuperAdminIdentityUserRole = new IdentityUserRole<string>()
            {
                UserId = superAdminAccount.Id,
                RoleId = AdminRole.Id
            };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(SuperAdminIdentityUserRole);
            #endregion

            #region RolePremissions
            List<IdentityRoleClaim<string>> SuperAdminClaimsList = new();
            foreach (var permision in Permissions.GetAllPermissionsList()) 
            {
                SuperAdminClaimsList.Add(
                   new IdentityRoleClaim<string>()
                   {
                       RoleId = AdminRole.Id,
                       ClaimType = "Permission",
                       ClaimValue= permision

                   });
            }

                     
            List<IdentityRoleClaim<string>> AllRolePermisionsList = new();
            AllRolePermisionsList.AddRange(SuperAdminClaimsList);
            for (int i = 0; i < AllRolePermisionsList.Count(); i++)
            {
                AllRolePermisionsList[i].Id = i+1;
            }
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(AllRolePermisionsList);

            #endregion

        }
    }
}

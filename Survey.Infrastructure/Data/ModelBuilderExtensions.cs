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
            IdentityUser defaultUser = new IdentityUser
            {
                Id = "2",
                UserName = "user",
                NormalizedEmail = "user@gmail.com".ToUpper(),
                Email = "user@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            defaultUser.PasswordHash = hasher.HashPassword(defaultUser, "P@ssw0rd");

            modelBuilder.Entity<IdentityUser>().HasData(superAdminAccount,defaultUser);
            #endregion

            #region RolesSeed
            IdentityRole SuperAdminRole = new IdentityRole()
            {
                Id = "1",
                Name = Roles.SuperAdmin.ToString(),
                NormalizedName = Roles.SuperAdmin.ToString().ToUpper()
            };
            IdentityRole AdminRole = new IdentityRole()
            {
                Id = "2",
                Name = Roles.Dean.ToString(),
                NormalizedName = Roles.Dean.ToString().ToUpper()
            };
            IdentityRole StudentRole = new IdentityRole()
            {
                Id = "5",
                Name = Roles.Student.ToString(),
                NormalizedName = Roles.Student.ToString().ToUpper()
            };
            IdentityRole QARole = new IdentityRole()
            {
                Id = "4",
                Name = Roles.QA.ToString(),
                NormalizedName = Roles.QA.ToString().ToUpper()
            };
            IdentityRole ProfessorRole = new IdentityRole()
            {
                Id = "3",
                Name = Roles.Professor.ToString(),
                NormalizedName = Roles.Professor.ToString().ToUpper()
            };
            modelBuilder.Entity<IdentityRole>().HasData(SuperAdminRole, AdminRole,StudentRole,ProfessorRole,QARole);
            #endregion

            #region IdentityUserRole
            
            IdentityUserRole<string> SuperAdminIdentityUserRole = new IdentityUserRole<string>()
            {
                UserId = superAdminAccount.Id,
                RoleId = SuperAdminRole.Id
            };
            IdentityUserRole<string> DefaultUserRole = new IdentityUserRole<string>()
            {
                UserId = defaultUser.Id,
                RoleId = AdminRole.Id
            };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(SuperAdminIdentityUserRole,DefaultUserRole);
            #endregion

            #region RolePremissions
            List<IdentityRoleClaim<string>> SuperAdminClaimsList = new();
            foreach (var permision in Permissions.GetAllPermissionsList())
            {
                SuperAdminClaimsList.Add(
                   new IdentityRoleClaim<string>()
                   {
                       RoleId = SuperAdminRole.Id,
                       ClaimType = "Permission",
                       ClaimValue = permision

                   });
            }

            List<IdentityRoleClaim<string>> AdminClaimsList = new();
            foreach (var permision in PremissionList.GetAllUserPermissionList())
            {
                AdminClaimsList.Add(
                   new IdentityRoleClaim<string>()
                   {
                       RoleId = AdminRole.Id,
                       ClaimType = "Permission",
                       ClaimValue = permision

                   });
            }
            List<IdentityRoleClaim<string>> StudentClaimList = new()
            {
                new IdentityRoleClaim<string>()
                {
                    RoleId = StudentRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = PremissionList.Survey_View,
                },
               
                new IdentityRoleClaim<string>()
                {
                    RoleId = StudentRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = PremissionList.Survey_Collect,
                }
            };
            List<IdentityRoleClaim<string>> AllRolePermisionsList = new();
            AllRolePermisionsList.AddRange(StudentClaimList);
            AllRolePermisionsList.AddRange(SuperAdminClaimsList);
            AllRolePermisionsList.AddRange(AdminClaimsList);
            for (int i = 0; i < AllRolePermisionsList.Count(); i++)
            {
                AllRolePermisionsList[i].Id = i + 1;
            }
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(AllRolePermisionsList);

            #endregion

        }
    }
}

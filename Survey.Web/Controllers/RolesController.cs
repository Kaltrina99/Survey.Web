using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Core.Constants;
using Survey.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _db = db;
        }
        [Authorize(Permissions.PremissionList.Role_View)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [Authorize(Permissions.PremissionList.Role_Add)]
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

            }
            return RedirectToAction("Index");
        }
        [Authorize(Permissions.PremissionList.Role_Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                var rols=_roleManager.FindByIdAsync(id);
                var r = _roleManager.GetClaimsAsync(rols.Result).Result;//.ToListAsync().Result;
              if(r != null)
                {
                    foreach (var c in r) {
                        _roleManager.RemoveClaimAsync(rols.Result, c);
                            }
}
               
                await _roleManager.DeleteAsync(rols.Result);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Permissions.PremissionList.Role_Update)]
        public async Task<IActionResult> Update(string id)
        {
            IdentityRole user = await _roleManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [Authorize(Permissions.PremissionList.Role_Update)]
        [HttpPost]
        public async Task<IActionResult> Update(IdentityRole role)
        {
            IdentityRole user = await _roleManager.FindByIdAsync(role.Id);
            if (user != null)
            {
                if (user.Name != null)
                {
                    IdentityResult result = await _roleManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }
            else
                ModelState.AddModelError("", "Role Not Found");
            _db.SaveChanges();
            return View(user);
        }
    }
}

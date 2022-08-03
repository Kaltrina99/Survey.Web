using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Survey.Core.Interfaces;
using Survey.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionRole _roleClaim;

        public PermissionController(IPermissionRole roleClaim)
        {
            _roleClaim = roleClaim;
        }

        public async Task<IActionResult> Index(string roleId)
        {
            var model = new PermissionViewModel();
            var response = await _roleClaim.GetAllPermissionsForRole(roleId);

            model.RoleClaims = response.Data;
            return View(model);
        }

        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).Select(x => x.Value).ToList();
            var response = await _roleClaim.UpdatePermissionForRole(selectedClaims, model.RoleId);

            return RedirectToAction("Index", "Roles");
        }
    }
}

using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class ProjectCategoryController : Controller
    {
        private readonly IProjectCategory _projectCategory;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ProjectCategoryController(IProjectCategory projectCategory, UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _projectCategory = projectCategory;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<ProjectCategory> projectCategoryList = _projectCategory.GetAll();
            return View(projectCategoryList);
        }

        #region Create Project Category

        public IActionResult CreateProjectCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProjectCategory(ProjectCategory projectCategory)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _projectCategory.Add(projectCategory);
                _projectCategory.Save();
                return RedirectToAction("Index");
            }
            return View(projectCategory);
        }

        #endregion

        #region Update Category

        public IActionResult UpdateCategory(int? id)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);

            ProjectViewModel model = new ProjectViewModel()
            {
                ProjectCategory = new ProjectCategory()
            };

            if (id == null || id == 0)
            {
                return View(model);
            }
            else
            {
                model.ProjectCategory = _projectCategory.Find(id.GetValueOrDefault());
                if (model.ProjectCategory == null)
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(ProjectViewModel projectViewModel)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _projectCategory.Update(projectViewModel.ProjectCategory);
                TempData[WebConstants.Success] = "Action Completed Successfully !";
                _projectCategory.Save();

                return RedirectToAction("Index");
            }
            return View(projectViewModel);
        }

        #endregion

        #region Delete Category

        public IActionResult DeleteCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProjectCategory category = _projectCategory.FirstOrDefault(b => b.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteExistingCategory(int? id)
        {
            var category = _projectCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            _projectCategory.Remove(category);
            _projectCategory.Save();
            TempData[WebConstants.Success] = "Action Completed Successfully !";

            return RedirectToAction("Index");
        }

        #endregion

        #region Enroll Users

        public IActionResult EnrollUsersToClients(string id)
        {
            ViewBag.u = id;

            var u = _userManager.GetUserAsync(HttpContext.User);
            var t = _dbContext.ProjectCategories.FirstOrDefault(x => (x.Id).ToString() == id /*&& x.Tenant_Id == ten.Id*/);
            if (t == null)
            {
                ViewBag.ErrorMessage = $"Team with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new List<ProjectViewModel>();


            return View(model);
        }
        #endregion

    }
}

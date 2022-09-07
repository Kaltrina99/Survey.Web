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
        public IActionResult Enroll(int id)
        {
            ViewBag.u = id;

            var u = _userManager.GetUserAsync(HttpContext.User);
            var t = _dbContext.ProjectCategories.FirstOrDefault(x => x.Id == id);
          
            var model = new List<CategoryPartUserViewModel>();
            var us = _dbContext.Users.ToList();

            foreach (var user in us)
            {
                var tamPartUserViewModel = new CategoryPartUserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                var d = _dbContext.UserProjectCategories.FirstOrDefault(x => x.UserId == user.Id && x.CategoryId == t.Id);
                if (d != null)
                {
                    tamPartUserViewModel.IsSelected = true;
                }
                else
                {
                    tamPartUserViewModel.IsSelected = false;
                }

                model.Add(tamPartUserViewModel);
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult Enroll(List<CategoryPartUserViewModel> model, int id)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            var t = _dbContext.ProjectCategories.FirstOrDefault(x => x.Id == id );
            if (t == null)
            {
                ViewBag.ErrorMessage = $"Team with Id = {id} cannot be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == model[i].UserId);

                var d = _dbContext.UserProjectCategories.AsNoTracking().FirstOrDefault(x => x.UserId == user.Id && x.CategoryId == t.Id);
                if (model[i].IsSelected && !(d != null))
                {
                    UserProjectCategory model1 = new UserProjectCategory()
                    {
                        UserId = user.Id,
                        CategoryId = t.Id
                    };
                    var result = _dbContext.UserProjectCategories.Add(model1);
                    _dbContext.SaveChanges();
                }
                else if (!model[i].IsSelected && d != null)
                {
                    UserProjectCategory model1 = new UserProjectCategory()
                    {
                        UserId = user.Id,
                        CategoryId = t.Id
                    };
                    var result = _dbContext.UserProjectCategories.Remove(model1);
                    _dbContext.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }
        //public async Task<IActionResult> RemoveUser(string id, string team)
        //{
        //    if (id == null && team != null)
        //    {
        //        return RedirectToAction("Index");

        //    }
        //    var response = await _projectCategory.RemoveUserFromTeamAsync(team, id);
        //    return RedirectToAction("Index");
        //}
        //[Authorize(Permissions.PremissionList.Team_View)]
        //public IActionResult GetUserInTeam(int id)
        //{

        //    var usersIds = _dbContext.UserProjectCategories.Where(x => x.CategoryId == id).Select(x => x.UserId);
        //    var users = _dbContext.Users.Where(x => usersIds.Any(y => y == x.Id)).ToList();
        //    var te = _dbContext.ProjectCategories.FirstOrDefault(x => x.Id == id);
        //    ViewBag.name = te.Name;
        //    ViewBag.u = id;
        //    //var model = new ProjectCategory()
        //    //{
        //    //    UserList = users
        //    //};
        //    return View(model);
        //}


    }
}

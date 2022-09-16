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
using Survey.Core.Constants;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Permissions.PremissionList.Category_View)]
        public IActionResult Index(int? id)
        {
            ViewData["Id"] = id;
            var u = _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<ProjectCategory> projectCategoryList = _projectCategory.GetAll();
            var parent = projectCategoryList.Where(x => x.ParentID==id);
            return View(parent);
        }

        public IActionResult Sub()
        {
            return View();
        }
        
        //public  IActionResult SubCategory()
        //{
        //    //IList<ProjectCategory> comments = _dbContext.ProjectCategories.Where(c => c.ParentID == null).ToList();
        //    //return View(comments);
        //    return View();
        //}

        #region Create Project Category
        [Authorize(Permissions.PremissionList.Category_Add)]
        public IActionResult CreateProjectCategory(int? id)
        {
            ProjectCategory model = new ProjectCategory();
            if (id == null || id == 0)
            {
                model.ParentID =null;
                return View(model);
            }
            else
            {
                model.ParentID = id;
                return View(model);
            }
        }
        [Authorize(Permissions.PremissionList.Category_Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProjectCategory(ProjectCategory projectCategory)
        {
            projectCategory.Id = 0;
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
        [Authorize(Permissions.PremissionList.Category_Update)]
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
        [Authorize(Permissions.PremissionList.Category_Update)]
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
        [Authorize(Permissions.PremissionList.Category_Delete)]
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

        [Authorize(Permissions.PremissionList.Category_Delete)]
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteExistingCategory(int? id)
        {
            var category = _projectCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            var cToDelete = SubCat(category.Id);
            cToDelete.Add(category);
            _dbContext.ProjectCategories.RemoveRange(cToDelete);
            _dbContext.SaveChanges();
            //_projectCategory.Remove(category);
            //_projectCategory.Save();
            TempData[WebConstants.Success] = "Action Completed Successfully !";

            return RedirectToAction("Index");
        }
        public List<ProjectCategory> SubCat(int folderId)
        {
            var subFolders = _dbContext.ProjectCategories
                .Where(d => d.ParentID == folderId)
                .ToList();

            var allFolders = new List<ProjectCategory>();
            foreach (var subFolder in subFolders)
            {
                allFolders.Add(subFolder);
                allFolders.AddRange( SubCat(subFolder.Id));
            }

            return allFolders;
        }
        #endregion

        #region Enroll Users
        [Authorize(Permissions.PremissionList.Category_AssignUser)]
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
        [Authorize(Permissions.PremissionList.Category_AssignUser)]
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
        [Authorize(Permissions.PremissionList.Category_AssignUser)]
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
       
    }
}

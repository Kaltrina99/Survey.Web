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
using NPOI.SS.Formula.Functions;
using Microsoft.CodeAnalysis;

namespace Survey.Web.Controllers
{
    public class ProjectCategoryController : Controller
    {
        private readonly IProjectCategory _projectCategory;
        private readonly IProjects _projects;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ProjectCategoryController(IProjectCategory projectCategory, IProjects projects,UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _projects = projects;
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
            var parent = projectCategoryList.Where(x => x.ParentID == id);
            return View(parent);
        }

        public IActionResult Sub()
        {
            return View();
        }

        #region Create Project Category
        [Authorize(Permissions.PremissionList.Category_Add)]
        public IActionResult CreateProjectCategory(int? id)
        {
            ProjectCategory model = new ProjectCategory();
            if (id == null || id == 0)
            {
                model.ParentID = null;
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
            var idS = projectCategory.ParentID;
           projectCategory.Id = 0;
           
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _projectCategory.Add(projectCategory);
                _projectCategory.Save();
                var us = _dbContext.UserProjectCategories.Where(x => x.CategoryId == idS);
                foreach (var item in us)
                {
                    UserProjectCategory model1 = new UserProjectCategory()
                    {
                        UserId = item.UserId,
                        CategoryId = projectCategory.Id
                    };
                    _dbContext.UserProjectCategories.Add(model1);
                    

                }
                try
                {
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("Cannot insert duplicate key row in object") ?? false)
                {
                    //We know that the actual exception was a duplicate key row
                }
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
            var p=_dbContext.Projects.Where(x => x.ProjectCategoryId == id).ToList();
            foreach (var item in p)
            {
                _projects.Remove(item);
                var f=_dbContext.Forms.Where(x => x.Project_Id == item.Id).ToList();
                foreach (var q in f)
                {
                    var qu=_dbContext.Questions.Where(x => x.Form_Id == q.Id).ToList();
                    foreach (var r in qu)
                    {
                        var sl = _dbContext.SkipLogic.Where(x => x.Parent_Question_Id == r.Id).ToList();
                        _dbContext.SkipLogic.RemoveRange(sl);
                    }
                    _dbContext.Questions.RemoveRange(qu);
                    var aw=_dbContext.Answers.Where(x=>x.Form_Id == q.Id).ToList();
                    _dbContext.Answers.RemoveRange(aw);
                }
               _dbContext.SaveChanges();
               // _projects.Save();
            }
            
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
                allFolders.AddRange(SubCat(subFolder.Id));
            }

            return allFolders;
        }
        #endregion

        #region Enroll Users
     
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
            var t = _dbContext.ProjectCategories.FirstOrDefault(x => x.Id == id);
            if (t == null)
            {
                ViewBag.ErrorMessage = $"Team with Id = {id} cannot be found";
                return View("NotFound");
            }


            for (int i = 0; i < model.Count; i++)
            {
                var list = PreorderCategories(id, t.Id);
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
                    foreach (var c in list)
                    {
                        var check = _dbContext.UserProjectCategories.Any(x=>x.UserId==user.Id && x.CategoryId==c.Item1);
                        if (!check)
                        {
                            UserProjectCategory modelchild = new UserProjectCategory()
                            {
                                UserId = user.Id,
                                CategoryId = c.Item1
                            };
                            _dbContext.UserProjectCategories.Add(modelchild);
                        }
                      
                    }
                    try
                    {
                        _dbContext.SaveChanges();
                    }
                    catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("Cannot insert duplicate key row in object") ?? false)
                    {
                        //We know that the actual exception was a duplicate key row
                    }
                }
                else if (!model[i].IsSelected && d != null)
                {
                    UserProjectCategory model1 = new UserProjectCategory()
                    {
                        UserId = user.Id,
                        CategoryId = t.Id
                    };
                    var result = _dbContext.UserProjectCategories.Remove(model1);
                    foreach (var c in list)
                    {
                        var check = _dbContext.UserProjectCategories.Any(x => x.UserId == user.Id && x.CategoryId == c.Item1);
                        if (check)
                        {
                            UserProjectCategory modelchild = new UserProjectCategory()
                            {
                                UserId = user.Id,
                                CategoryId = c.Item1
                            };
                            _dbContext.UserProjectCategories.Remove(modelchild);
                        }

                    }
                    try
                    {
                        _dbContext.SaveChanges();
                    }
                    catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("Cannot insert duplicate key row in object") ?? false)
                    {
                        //We know that the actual exception was a duplicate key row
                    }
                }




            }

            return RedirectToAction("Index");
        }

        List<Tuple<int, string>> PreorderCategories(int id, int? p)
        {
            var data = _dbContext.ProjectCategories.Include(x => x.Childs).AsNoTracking().Where(x => x.Id == p).ToList();
            return PreorderCategories(data, p);
        }
        List<Tuple<int, string>> PreorderCategories(List<ProjectCategory> categories, int? parentID)
        {
            var result = new List<Tuple<int, string>>();
           
            var children = _dbContext.ProjectCategories.Include(x => x.Childs).AsNoTracking().Where(x => x.ParentID == parentID).ToList();//categories.Where(c => c.Id == parentID);
            foreach (var category in children)
            {
                result.Add(new Tuple<int, string>(category.Id, category.Name));
                result.AddRange(PreorderCategories(categories, category.Id));
            }
            return result;
        }
    }
}

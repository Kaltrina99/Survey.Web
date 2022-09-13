using Survey.Core.Constants;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjects _project;
        private readonly IProjectCategory _projectCategory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public ProjectController(IProjects project, IWebHostEnvironment webHostEnvironment, IProjectCategory projectCategory, UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _project = project;
            _webHostEnvironment = webHostEnvironment;
            _projectCategory = projectCategory;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [Authorize(Permissions.PremissionList.Project_View)]

        public IActionResult Index(int? projectId, string searchingword)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
       

            ProjectViewModel indexViewModel = new ProjectViewModel()
            {
               Projects = _project.GetAll(includeProperties: "ProjectCategory").ToList(),
            };
            return View(indexViewModel);
        }

        #region Create Project
        [Authorize(Permissions.PremissionList.Project_Add)]

        public IActionResult CreateProject()
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            ProjectViewModel projectViewModel = new ProjectViewModel()
            {
                Project = new Projects(),
                ProjectCategorySelectList = _project.GetAllDropdownList(WebConstants.Category)
            };
            return View(projectViewModel);
        }
        [Authorize(Permissions.PremissionList.Project_Add)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProject(ProjectViewModel projectViewModel)
        {
          
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _project.Add(projectViewModel.Project);
                _project.Save();


                return RedirectToAction("Index");
            }
            return View(projectViewModel);
        }

        #endregion

        #region Update Project
        [Authorize]
        public IActionResult UpdateProject(int? id)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            ProjectViewModel projectViewModel = new ProjectViewModel()
            {
                Project = new Projects(),
                ProjectCategorySelectList = _project.GetAllDropdownList(WebConstants.Category)
            };

            if (id == null || id == 0)
            {
                return View(projectViewModel);
            }
            else 
            {
                projectViewModel.Project = _project.Find(id.GetValueOrDefault());
                if (projectViewModel.Project == null)
                {
                    return NotFound();
                }
            }
            return View(projectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProject(ProjectViewModel projectViewModel)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _project.Update(projectViewModel.Project);
                TempData[WebConstants.Success] = "Action Completed Successfully !";
                _project.Save();

                return RedirectToAction("Index");
            }
            projectViewModel.ProjectCategorySelectList = _project.GetAllDropdownList(WebConstants.Category);

            return View(projectViewModel);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Delete Project

        public IActionResult DeleteProject(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Projects project = _project.FirstOrDefault(b => b.Id == id, includeProperties: "ProjectCategory");

            if (project == null)
            {
                return NotFound();
            }
            _project.Remove(project);
            _project.Save();

            return RedirectToAction("Index");

        }
        #endregion

    }
}

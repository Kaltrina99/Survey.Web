using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class SurveyResultsController : Controller
    {
        private readonly ISurveyResults _results;
        private readonly ISurveyResultDownload _download;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        public SurveyResultsController(ISurveyResults results, ISurveyResultDownload download, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _results = results;
            _download = download;
            _env = env;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index(int id)
        {
            var response = await _results.getDashbord(id);
            if (!response.Success) 
            {
                return BadRequest(response.Message);
            }
            return View(response.Data);
        }
        #endregion

        #region Table
        public async Task<IActionResult> Table(int id,int pageSize=10,int pageNumber=1) {
             var response = await _results.getTable(id,pageNumber,pageSize);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return View(response.Data);
        }
        #endregion

      
        #region Download
        [Authorize]
        public async Task<IActionResult> Downloads(int id)
        {
            SurveyResultsViewModel model = new();
            
                var response = await _download.getDownloads(id);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
            model = response.Data;
            model.FormId = id;
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> NewDownload(int id,int option)
        {
            string userid= (await _userManager.GetUserAsync(HttpContext.User))?.Id;
            var response = await _download.NewDownload(id,option, _env.WebRootPath, userid);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return PartialView(response.Data.Download);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            string rootpath = _env.WebRootPath;
            var response = await _download.DownloadFile(id,rootpath);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            string path = response.Data;
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/force-download", Path.GetFileName(path));
        }
        #endregion

        #region

        public async Task<IActionResult> Reports(int id) 
        {
            SurveyResultsViewModel model = new();
            model.FormId = id;
            var response = await _results.getReports(id);
            if (!response.Success)
            {
                return BadRequest();
            }
            model = response.Data;
            return View(model) ;
        }

        #endregion
    }
}

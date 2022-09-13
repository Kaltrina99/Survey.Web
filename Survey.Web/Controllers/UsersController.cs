using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using Survey.Core.Constants;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Survey.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;

        // private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        readonly ApplicationDbContext _dbContext;
        public UsersController(IConfiguration configuration,UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)// IEmailSender emailSender)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
            // _emailSender = emailSender;
        }
        [Authorize(Permissions.PremissionList.User_ViewUsers)]

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUuer = await _userManager.Users.ToListAsync();
            return View(allUsersExceptCurrentUuer);
        }
        [Authorize(Permissions.PremissionList.User_AddUser)]

        public ViewResult Create() => View();
        [Authorize(Permissions.PremissionList.User_AddUser)]

        [HttpPost]
        public async Task<IActionResult> Create(UsersViewModel user)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            if (ModelState.IsValid)
            {
                IdentityUser appUser = new IdentityUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                var usCheck = _dbContext.Users.Any(x => x.Email == user.Email);
                // student. = row.Cell(3).Value.ToString();
                if (!usCheck)//student.Id == Guid.Empty)
                {
                    _dbContext.Users.Add(appUser);
                    _dbContext.SaveChanges();
                    var idu = _dbContext.Users.FirstOrDefault(x => x.Email == appUser.Email).Id;
                    var adminRole = _dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == "student");
                    IdentityUserRole<string> iur = new IdentityUserRole<string>
                    {
                        RoleId = adminRole.Id,
                        UserId = idu //user.Id
                    };
                    var ut = _dbContext.UserRoles.Add(iur);
                    _dbContext.SaveChanges();
                }
               // string temporary_sender = user.Email;
               //string Link = HttpContext.Request.Host + "/Identity/Account/Login?ReturnUrl=%2F?" ;

                //_emailSender.SendEmailAsync(temporary_sender, "Welcome new user:" + user.Name, "Username: " + user.Name + "<br><br>Password: " + user.Password + "<br><br>Emal: " + user.Email + $"<br><br>Try it now <a href='{HtmlEncoder.Default.Encode(Link)}'>login here</a>.");
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        [Authorize(Permissions.PremissionList.User_UpdateUser)]

        public async Task<IActionResult> Update(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [Authorize(Permissions.PremissionList.User_UpdateUser)]

        [HttpPost]
        public async Task<IActionResult> Update(IdentityUser userUpdate, string password)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userUpdate.Id);
            if (user != null)
            {
                if (userUpdate.Email != null)
                {
                    user.Email = userUpdate.Email;
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
                        user.PasswordHash = hasher.HashPassword(user, password);
                    }
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }
        [Authorize(Permissions.PremissionList.User_DeleteUser)]

        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                IdentityUser a = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(a);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Permissions.PremissionList.User_AddUser)]
        public  ActionResult ImportUsers()
        {
            return View();
        }

        [Authorize(Permissions.PremissionList.User_AddUser)]
        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        {

            try
            {
                var fileextension = Path.GetExtension(FormFile.FileName);
                var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadsUsers", filename);
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    FormFile.CopyTo(fs);
                }
                int rowno = 1;
                XLWorkbook workbook = XLWorkbook.OpenFromTemplate(filepath);
                var sheets = workbook.Worksheets.First();
                var rows = sheets.Rows().ToList();
                foreach (var row in rows)
                {
                    if (rowno != 1)
                    {
                        var test = row.Cell(1).Value.ToString();
                        if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                        {
                            break;
                        }
                        IdentityUser student;
                        student = _dbContext.Users.Where(s => s.UserName == row.Cell(1).Value.ToString()).FirstOrDefault();
                        if (student == null)
                        {
                            student = new IdentityUser();
                        }
                        student.UserName = row.Cell(1).Value.ToString();
                        student.Email = row.Cell(2).Value.ToString();
                        student.EmailConfirmed = true;
                        student.NormalizedUserName=row.Cell(1).Value.ToString().ToLower();
                        student.NormalizedEmail = row.Cell(2).Value.ToString().ToUpper();
                        IdentityResult result = await _userManager.CreateAsync(student, "P@ssw0rd");
                        
                        var usCheck = _dbContext.Users.Any(x=>x.Email== row.Cell(2).Value.ToString());
                        
                        if (!usCheck)
                        {
                            _dbContext.Users.Add(student);
                            UserProjectCategory p= new UserProjectCategory();
                       
                            _dbContext.SaveChanges();
                            var idu = _dbContext.Users.FirstOrDefault(x => x.Email == row.Cell(2).Value.ToString()).Id;
                            p.UserId = idu;
                            p.CategoryId =int.Parse(row.Cell(4).Value.ToString());
                            _dbContext.UserProjectCategories.Add(p);
                            
                            IdentityUserRole<string> iur = new IdentityUserRole<string>
                            {
                                RoleId = row.Cell(3).Value.ToString(),
                                UserId = idu //user.Id
                            };
                            var ut = _dbContext.UserRoles.Add(iur);
                        }
                        else
                            _dbContext.Users.Update(student);
                    }
                    else
                    {
                        rowno = 2;
                    }
                }
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
                
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        [HttpPost]
        public IActionResult ExportUser()
        {
            DataTable dt = new DataTable("UsersExcel");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Name"),
                                        new DataColumn("Email"),
                                        new DataColumn("RoleId"),
                                        new DataColumn("CategoryId") });

           
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersExcel.xlsx");
                }
            }
        }

    }
}

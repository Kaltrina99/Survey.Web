using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Survey.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        readonly ApplicationDbContext _dbContext;
        public UsersController(IConfiguration configuration,UserManager<IdentityUser> userManager, ApplicationDbContext dbContext,IEmailSender emailSender)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailSender = emailSender;
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
                var DefaultPassword = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["DefaultPassword"];
                var DefaultEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["DefaultEmail"];
                var DefaultLink = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["DefaultLink"];
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
                        var exist = _dbContext.Users.Any(x => x.Email == row.Cell(1).Value.ToString());
                        if(!exist)
                        {
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
                            student.Email = row.Cell(1).Value.ToString();
                            student.EmailConfirmed = true;
                            student.NormalizedUserName = row.Cell(1).Value.ToString().ToUpper();
                            student.NormalizedEmail = row.Cell(1).Value.ToString().ToUpper();
                            IdentityResult result = await _userManager.CreateAsync(student, DefaultPassword);

                            if (!student.Email.Contains(DefaultEmail))
                            {
                                var email = _emailSender.SendEmailAsync(student.Email, "New user", $"<br><br>Pershendetje, <br/>Sapo jeni shtuar si perdorues ne RiVlersim, aplikacion ky nga kolegji Riinvest per krijimin e anketave.<br/>Kliko <a href={HtmlEncoder.Default.Encode(DefaultLink)}'> ketu</a> per te vazhdua ne platformen RiVlersim <br/> Ju mund te qasini me keto kredenciale <br/>Email: {student.Email} <br/>Password: {DefaultPassword}");
                            }
                            else
                            {
                                var email = _emailSender.SendEmailAsync(student.Email, "New user", $"<br><br>Pershendetje, <br/>Sapo jeni shtuar si perdorues ne RiVlersim, aplikacion ky nga kolegji Riinvest per krijimin e anketave.<br/>Kliko <a href={HtmlEncoder.Default.Encode(DefaultLink)}'> ketu</a> per te vazhdua ne platformen RiVlersim <br/> Ju mund te qasini me keto kredenciale <br/>Email: {student.Email} <br/>Password: {DefaultPassword} <br/> Apo permes Google Account Authentification qe ju eshte ofruar nga Kolegji Riinvest ");
                            }
                            var usCheck = _dbContext.Users.Any(x => x.Email == row.Cell(1).Value.ToString());
                            if (usCheck)
                            {
                                var idu = _dbContext.Users.FirstOrDefault(x => x.Email == row.Cell(1).Value.ToString()).Id;
                                if (!String.IsNullOrEmpty(row.Cell(6).Value.ToString()))
                                {
                                    UserProjectCategory p = new UserProjectCategory();

                                    p.UserId = idu;

                                    p.CategoryId = int.Parse(row.Cell(6).Value.ToString());
                                    _dbContext.UserProjectCategories.Add(p);
                                }
                                if (!String.IsNullOrEmpty(row.Cell(5).Value.ToString()))
                                {
                                    UserProject p = new UserProject();

                                    p.UserId = idu;

                                    p.ProjectsId = int.Parse(row.Cell(5).Value.ToString());
                                    _dbContext.UserProject.Add(p);
                                }
                                if (!String.IsNullOrEmpty(row.Cell(4).Value.ToString()))
                                {
                                    IdentityUserRole<string> iur = new IdentityUserRole<string>
                                    {
                                        RoleId = row.Cell(4).Value.ToString(),
                                        UserId = idu //user.Id
                                    };
                                    var ut = _dbContext.UserRoles.Add(iur);
                                }
                            } 
                        }
                        else {
                            IdentityUser user= _dbContext.Users.Where(s => s.UserName == row.Cell(1).Value.ToString()).FirstOrDefault();
                            _dbContext.Users.Update(user);
                        }
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
            dt.Columns.AddRange(new DataColumn[4] {
                                        new DataColumn("Email"),
                                        new DataColumn("RoleId"),
                                        new DataColumn("ProjectId"),
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
        public async Task<IActionResult> ChangePassword(string id)
        {
            ChangePassword user = new ChangePassword();
               user.UserId =  _dbContext.Users.FirstOrDefault(x => x.UserName == id).Id;
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword userUpdate)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userUpdate.UserId);
            ChangePassword newPage = new ChangePassword();
            if (user != null)
            {
                if (userUpdate.NewPassword == userUpdate.ConfirmPassword)
                {
                   
                    if (!string.IsNullOrWhiteSpace(userUpdate.NewPassword))
                    {
                        PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
                        user.PasswordHash = hasher.HashPassword(user, userUpdate.NewPassword);
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
                        newPage.UserId = userUpdate.UserId;
                        return View(newPage);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Check password again!");
                    
                    newPage.UserId=userUpdate.UserId;
                    return View(newPage);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return RedirectToAction("Index");
            }
            return View(user);
        }


    }
}

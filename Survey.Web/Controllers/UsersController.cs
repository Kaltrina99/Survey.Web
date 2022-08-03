using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Web.Controllers
{
    public class UsersController : Controller
    {
       // private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        readonly ApplicationDbContext _dbContext;
        public UsersController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)// IEmailSender emailSender)
        {
            _userManager = userManager;
            _dbContext = dbContext;
           // _emailSender = emailSender;
        }
       // [Authorize(Permissions.PremissionList.User_ViewUsers)]

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUuer = await _userManager.Users.ToListAsync();
            return View(allUsersExceptCurrentUuer);
        }
        //[Authorize(Permissions.PremissionList.User_AddUser)]

        public ViewResult Create() => View();
       // [Authorize(Permissions.PremissionList.User_AddUser)]

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
                var idu = _dbContext.Users.FirstOrDefault(x => x.Email == user.Email).Id;
                var adminRole = _dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == "user");
                IdentityUserRole<string> iur = new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = idu //user.Id
                };
                var ut = _dbContext.UserRoles.Add(iur);
                _dbContext.SaveChanges();
                string temporary_sender = user.Email;
                string Link = HttpContext.Request.Host + "/Identity/Account/Login?ReturnUrl=%2F?" ;

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
        //[Authorize(Permissions.PremissionList.User_UpdateUser)]

        public async Task<IActionResult> Update(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        //[Authorize(Permissions.PremissionList.User_UpdateUser)]

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
       // [Authorize(Permissions.PremissionList.User_DeleteUser)]

        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                IdentityUser a = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(a);
            }
            return RedirectToAction("Index");
        }

    }
}

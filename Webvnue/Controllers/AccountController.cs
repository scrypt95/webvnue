using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<Models.MyIdentityUser> userManager;
        private RoleManager<Models.MyIdentityRole> roleManager;


        // GET: Account
        [Authorize]
        public ActionResult Index()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (userManager.IsInRole(user.Id, "Administrator"))
            {
                ViewData["UserInfo"] = "You're an Administrator!";
            }

            if (userManager.IsInRole(user.Id, "User"))
            {
                ViewData["UserInfo"] = "You're a regular user!";
            }
            if (user != null)
            {
                ViewData["FirstName"] = user.FirstName;
                ViewData["CurrentUser"] = user;
            }

            ViewBag.FullName = user.FirstName;

            return View();
        }

        private Models.MyIdentityUser getCurrentUser()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();
            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            UserManager<Models.MyIdentityUser> userManager = new UserManager<Models.MyIdentityUser>(userStore);

            Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

            return user;
        }

        public AccountController()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();

            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            userManager = new UserManager<Models.MyIdentityUser>(userStore);

            RoleStore<Models.MyIdentityRole> roleStore = new RoleStore<Models.MyIdentityRole>(db);
            roleManager = new RoleManager<Models.MyIdentityRole>(roleStore);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.Register registerModel)
        {
            if (ModelState.IsValid)
            {
                Models.MyIdentityUser user = new Models.MyIdentityUser();

                user.UserName = registerModel.UserName;
                user.Email = registerModel.Email;
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;
                user.BirthDate = registerModel.BirthDate;

                IdentityResult result = userManager.Create(user, registerModel.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("UserName", "Error while creating the user!");
                }
            }
            return View(registerModel);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.Login loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Models.MyIdentityUser user = userManager.Find(loginModel.UserName, loginModel.Password);

                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                    ClaimsIdentity identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = loginModel.RememberMe;

                    authenticationManager.SignIn(props, identity);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(loginModel);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Models.ChangePassword changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

                IdentityResult result = userManager.ChangePassword(user.Id, changePasswordModel.OldPassword, changePasswordModel.NewPassword);

                if (result.Succeeded)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Error while changing the password");
                }
            }

            return View(changePasswordModel);
        }

        [Authorize]
        public ActionResult ChangeProfile()
        {
            Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

            Models.ChangeProfile changeProfileModel = new Models.ChangeProfile();

            user.FirstName = changeProfileModel.FirstName;
            user.LastName = changeProfileModel.LastName;
            user.BirthDate = changeProfileModel.BirthDate;

            return View(changeProfileModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(Models.ChangeProfile changeProfileModel)
        {
            if (ModelState.IsValid)
            {
                Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);
                user.FirstName = changeProfileModel.FirstName;
                user.LastName = changeProfileModel.LastName;
                user.BirthDate = changeProfileModel.BirthDate;

                IdentityResult result = userManager.Update(user);

                if (result.Succeeded)
                {
                    ViewBag.Message = "Profile Updated Successfully!";
                }
                else
                {
                    ModelState.AddModelError("", "Error while saving profile.");
                }
            }

            return View(changeProfileModel);
        }

        [HttpGet]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ConfirmEmail(string Token, string Email)
        {
            Models.MyIdentityUser user = userManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.EmailConfirmed = true;
                    IdentityResult result = userManager.Update(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ConfirmEmail() {
            Models.MyIdentityUser user = getCurrentUser();

            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("webvnue@gmail.com", "Webvnue"), new System.Net.Mail.MailAddress(user.Email));
            m.Subject = "Email Confirmation";
            m.Body = string.Format("Dear {0}, <br/> Thank you for your registration, Click on the below link to complete your registration: <br/> <a href =\"{1}\" title =\"User Email Confirm\">{1}</a>", user.FirstName, Url.Action("ConfirmEmail", "Account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme)) ;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("webvnue@gmail.com", "#Iloveandy951");
            smtp.EnableSsl = true;
            smtp.Send(m);

            return Json(new{});
        }
    }
}
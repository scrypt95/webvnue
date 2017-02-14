using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class accountController : Controller
    {
        private UserManager<Models.MyIdentityUser> userManager;
        private RoleManager<Models.MyIdentityRole> roleManager;

        [Authorize]
        public ActionResult settings()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

            List<Models.MyIdentityUser> referralList = getReferralList(user);

            if (referralList.Count > 0)
            {
                ViewData["ReferralList"] = getReferralList(user);
            }
            ViewData["ReferralListCount"] = referralList.Count;

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

        private List<Models.MyIdentityUser> getReferralList(Models.MyIdentityUser user)
        {
            List<Models.MyIdentityUser> referralList = new List<Models.MyIdentityUser>();

            var db = new Models.MyIdentityDbContext();

            foreach(var referral in db.Referrals)
            {
                if(user.Id == referral.ReferrerId)
                {
                    referralList.Add(userManager.FindById(referral.RefereeId));
                }
            }

            return referralList;
        }

        public accountController()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();

            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            userManager = new UserManager<Models.MyIdentityUser>(userStore);

            RoleStore<Models.MyIdentityRole> roleStore = new RoleStore<Models.MyIdentityRole>(db);
            roleManager = new RoleManager<Models.MyIdentityRole>(roleStore);

            var provider = new DpapiDataProtectionProvider("Webvnue");
            userManager.UserTokenProvider = new DataProtectorTokenProvider<Models.MyIdentityUser>(provider.Create("ResetPassword"));
        }

        public ActionResult register(string Token)
        {
            bool loggedIn = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (loggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            Models.MyIdentityUser user = userManager.FindById(Token);

            if (user != null)
            {
                ViewData["Referrer"] = user;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult register(Models.Register registerModel, string Token)
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

                    if (Token != null && validateToken(Token))
                    {
                        addNewReferral(user, Token);
                        sendEmail(userManager.FindById(Token), "Webvnue Referral Notification", string.Format("Dear, {0} <br/><br/> {1} has signed up under your referral! <br/><br/> Your monthly income has increased by $4.50. <br/><br/> Best Regards, <br/>Team Webvnue", userManager.FindById(Token).FirstName, user.FirstName));
                    }

                    sendEmail(user, "Webvnue Registration", string.Format("Dear, {0} <br/><br/> Thank you for joining Webvnue. <br/><br/> You're on your way to becoming your own boss. <br/><br/> Best Regards, <br/>Team Webvnue", user.FirstName));
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                }
            }
            if (Token != null && validateToken(Token))
            {
                ViewData["Token"] = Token;
            }
            else
            {
                ViewData["Token"] = "";
            }
            return View(registerModel);
        }

        private bool validateToken(string Token)
        {
            Models.MyIdentityUser user = userManager.FindById(Token);

            if(user != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void addNewReferral(Models.MyIdentityUser user, string Token)
        {
            var db = new Models.MyIdentityDbContext();

            Models.Referral newReferral = new Models.Referral();
            newReferral.Id = Guid.NewGuid().ToString();
            newReferral.ReferrerId = userManager.FindById(Token).Id;
            newReferral.RefereeId = user.Id;

            db.Referrals.Add(newReferral);
            db.SaveChanges();
        }

        public ActionResult login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(Models.Login loginModel, string returnUrl)
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
                    ModelState.AddModelError("invalid", "Invalid username or password");
                }
            }

            return View(loginModel);
        }

        public ActionResult forgotpassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult forgotpassword(Models.forgotpassword model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmail(model.Email);
                if (user == null)
                {
                    // || !(userManager.IsEmailConfirmed(user.Id)
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("Index", "Home");
                }

                string code = userManager.GeneratePasswordResetToken(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                sendEmail(user, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult resetpassword(string code)
        {
            if(code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult resetpassword(Models.ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = userManager.FindByEmail(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Index", "Home");
            }
            var result = userManager.ResetPassword(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                sendEmail(user, "Password Changed", "Your password has been changed");
                return RedirectToAction("Index", "Home");
            }
            return View();
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
        public ActionResult logout()
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
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ConfirmEmail() {
            Models.MyIdentityUser user = getCurrentUser();

            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("webvnue@gmail.com", "Webvnue"), new System.Net.Mail.MailAddress(user.Email));
            m.Subject = "Email Confirmation";
            m.Body = string.Format("Dear {0}, <br/><br/> Thank you for joining Webvnue. <br/><br/> Click on the below link to confirm your email: <br/><br/> <a href =\"{1}\" title =\"User Email Confirm\">{1}</a>", user.FirstName, Url.Action("ConfirmEmail", "account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme)) ;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("webvnue@gmail.com", "#Iloveandy951");
            smtp.EnableSsl = true;
            smtp.Send(m);

            return null;
        }

        private void sendEmail(Models.MyIdentityUser user, string subject, string body)
        {
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("webvnue@gmail.com", "Webvnue"), new System.Net.Mail.MailAddress(user.Email));
            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("webvnue@gmail.com", "#Iloveandy951");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
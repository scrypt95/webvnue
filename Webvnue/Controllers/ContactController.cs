using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            Models.MyIdentityUser user = getCurrentUser();
            bool loggedIn = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (loggedIn)
            {
                ViewData["FirstName"] = user.FirstName;
                ViewData["UserName"] = user.UserName;
            }
            ViewData["LoggedIn"] = loggedIn;

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
    }
}
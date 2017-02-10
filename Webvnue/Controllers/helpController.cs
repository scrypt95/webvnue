using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Webvnue.Controllers
{
    public class helpController : Controller
    {
        // GET: help
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult contact()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

            return View();
        }

        public ActionResult faq()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

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
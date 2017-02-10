using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }
            return View();
        }
        [Route("{user}")]
        public ActionResult Personal(string user)
        {
            Models.MyIdentityUser userLoggedIn = getCurrentUser();

            if (userLoggedIn != null)
            {
                ViewData["CurrentUser"] = userLoggedIn;
            }

            Models.MyIdentityUser requestedUserPage = findUser(user);

            if(requestedUserPage != null)
            {
                return View();
            }
            else
            {
                return HttpNotFound("NOT FOUND MOTHER FUCKER");
            }
        }

        private Models.MyIdentityUser getCurrentUser()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();
            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            UserManager<Models.MyIdentityUser> userManager = new UserManager<Models.MyIdentityUser>(userStore);

            Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

            return user;
        }

        private Models.MyIdentityUser findUser(string userName)
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();
            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            UserManager<Models.MyIdentityUser> userManager = new UserManager<Models.MyIdentityUser>(userStore);

            Models.MyIdentityUser user = userManager.FindByName(userName);

            return user;
        }
    }
}
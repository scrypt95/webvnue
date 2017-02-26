using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class searchController : Controller
    {
        public ActionResult users(string query)
        {
            Models.MyIdentityUser userLoggedIn = getCurrentUser();

            if (userLoggedIn != null)
            {
                ViewData["CurrentUser"] = userLoggedIn;
            }

            if (query != null)
            {
                if (query.Length > 0)
                {
                    List<Models.MyIdentityUser> userList = getUserSearchList(query);

                    ViewData["UserList"] = userList;
                    //ViewData["UserImageList"] = getUserProfileImages(userList);
                    //ViewData["Dictionary"] = getUserXImageDictionary(userList);
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

            return View();
        }

        private List<Models.MyIdentityUser> getUserSearchList(string query)
        {
            var db = new Models.MyIdentityDbContext();

            List<Models.MyIdentityUser> userList = new List<Models.MyIdentityUser>();

            foreach(var user in db.Users)
            {
                if (user.FirstName.ToLower().Contains(query) || user.LastName.ToLower().Contains(query) || user.UserName.ToLower().Contains(query))
                {
                    userList.Add(user);
                }
            }

            return userList;
        }

        private List<Models.UserProfileImage> getUserProfileImages(List<Models.MyIdentityUser> userList)
        {
            var db = new Models.MyIdentityDbContext();

            List<Models.UserProfileImage> userImageList = new List<Models.UserProfileImage>();

            foreach (var user in userList)
            {
                Models.UserProfileImage img = db.UserProfileImages.Find(user.Id);

                if(img != null)
                {
                    userImageList.Add(img);
                }
            }

            return userImageList;
        }

        private Dictionary<Models.MyIdentityUser, Models.UserProfileImage> getUserXImageDictionary(List<Models.MyIdentityUser> userList)
        {
            var db = new Models.MyIdentityDbContext();

            Dictionary<Models.MyIdentityUser, Models.UserProfileImage> dict = new Dictionary<Models.MyIdentityUser, Models.UserProfileImage>();

            foreach(var user in userList)
            {
                dict.Add(user, db.UserProfileImages.Find(user.Id));
            }

            return dict;
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
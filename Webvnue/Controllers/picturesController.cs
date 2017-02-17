using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class picturesController : Controller
    {
        private UserManager<Models.MyIdentityUser> userManager;

        public picturesController()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();

            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            userManager = new UserManager<Models.MyIdentityUser>(userStore);
        }
        public ActionResult src()
        {
            ViewData["User"] = getCurrentUser();
            return View();
        }

        [HttpPost]
        public ActionResult src(HttpPostedFileBase[] uploadImages)
        {
            if (uploadImages.Count() <= 1)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var image in uploadImages)
            {
                if (image.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }
                    var userImage = new Models.UserProfileImage()
                    {
                        UserId = getCurrentUser().Id,
                        ImageData = imageData,
                        FileName = image.FileName
                    };

                    var db = new Models.MyIdentityDbContext();
                    db.UserProfileImages.Add(userImage);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult img(string id)
        {
            var db = new Models.MyIdentityDbContext();
            //Models.UserProfileImage userProfileImage = null;

            //foreach(var item in db.UserProfileImages)
            //{
            //    if(item.UserId == id)
            //    {
            //        userProfileImage = item;
            //    }
            //}

            var item = db.UserProfileImages.Find(id);
            byte[] buffer = item.ImageData;

            return File(buffer, "image/jpg", string.Format("{0}.jpg", id));
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
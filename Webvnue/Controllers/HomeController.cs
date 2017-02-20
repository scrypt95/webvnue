using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<Models.MyIdentityUser> userManager;

        public HomeController()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();

            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            userManager = new UserManager<Models.MyIdentityUser>(userStore);
        }

        // GET: Home
        //[RequireHttps]
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
                //ViewData["ReferralList"] = getReferralList(userLoggedIn);
            }

            Models.MyIdentityUser requestedUserPage = findUser(user);

            if(requestedUserPage != null)
            {
                ViewData["VisitedUser"] = requestedUserPage;
                ViewData["VisitedUserImages"] = getUserImageIdList(requestedUserPage);

                return View();
            }
            else
            {
                return HttpNotFound("NOT FOUND MOTHER FUCKER");
            }
        }

        [HttpPost]
        public ActionResult UploadProfileImage(HttpPostedFileBase[] uploadImage)
        {
            var db = new Models.MyIdentityDbContext();

            if (uploadImage.Length == 0)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            if (uploadImage[0] == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            if (db.UserProfileImages.Find(getCurrentUser().Id) != null){
                var profile = db.UserProfileImages.Find(getCurrentUser().Id);
                db.UserProfileImages.Remove(profile);
                db.SaveChanges();
            }

            foreach (var image in uploadImage)
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

                    db.UserProfileImages.Add(userImage);
                    db.SaveChanges();
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase[] uploadMainImage)
        {
            var db = new Models.MyIdentityDbContext();

            if (uploadMainImage.Length == 0)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            if (uploadMainImage[0] == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            foreach (var image in uploadMainImage)
            {

                if (image.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }
                    var userImage = new Models.UserImage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = getCurrentUser().Id,
                        ImageData = imageData,
                        FileName = image.FileName,
                        Rating = 0,
                        TimeStamp = DateTime.Now,
                        Views = 0
                        
                    };

                    db.UserImages.Add(userImage);
                    db.SaveChanges();
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult profileimg(string id)
        {
            var db = new Models.MyIdentityDbContext();

            var item = db.UserProfileImages.Find(id);
            byte[] buffer = item.ImageData;

            return File(buffer, "image/jpg", string.Format("{0}.jpg", id));
        }

        public ActionResult showImage(string id)
        {
            var db = new Models.MyIdentityDbContext();

            var image = db.UserImages.Find(id);
            byte[] buffer = image.ImageData;

            return File(buffer, "image/jpg", string.Format("{0}.jpg", id));


            /*
            List<FileContentResult> imageList = new List<FileContentResult>();

            foreach(var image in db.UserImages)
            {
                if(image.UserId == id)
                {
                    imageList.Add(new FileContentResult(image.ImageData, "image/jpg"));
                }
            }

            return imageList;
            */
        }

        private List<string> getUserImageIdList(Models.MyIdentityUser user)
        {
            var db = new Models.MyIdentityDbContext();

            List<string> userImageIdList = new List<string>();

            foreach(var obj in db.UserImages)
            {
                if (obj.UserId == user.Id)
                {
                    userImageIdList.Add(obj.Id);
                }
            }

            return userImageIdList;

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

        private List<Models.MyIdentityUser> getReferralList(Models.MyIdentityUser user)
        {
            List<Models.MyIdentityUser> referralList = new List<Models.MyIdentityUser>();

            var db = new Models.MyIdentityDbContext();

            foreach (var referral in db.Referrals)
            {
                if (user.Id == referral.ReferrerId)
                {
                    referralList.Add(userManager.FindById(referral.RefereeId));
                }
            }

            return referralList;
        }
    }
}
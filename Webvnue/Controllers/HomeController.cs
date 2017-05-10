using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
                ViewData["Followers"] = getFollowers(user.Id);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.Register registerModel, string Token)
        {
            if (ModelState.IsValid)
            {
                Models.MyIdentityUser user = new Models.MyIdentityUser();

                user.UserName = registerModel.UserName;
                user.Email = registerModel.Email;
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;
                user.BirthDate = registerModel.BirthDate;

                userManager.UserValidator = new UserValidator<Models.MyIdentityUser>(userManager)
                {
                    RequireUniqueEmail = true
                };

                IdentityResult result = userManager.Create(user, registerModel.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");

                    if (Token != null && validateToken(Token))
                    {
                        addNewReferral(user, Token);
                        sendEmail(userManager.FindById(Token), "Webvnue Referral Notification", string.Format("Dear {0}, <br/><br/> {1} has signed up under your referral! <br/><br/> Your monthly income has increased by $4.50. <br/><br/> Best Regards, <br/>Team Webvnue", userManager.FindById(Token).FirstName, user.FirstName));
                    }

                    //addDefaultPicture(user);
                    addUserDefaultProfileBio(user);
                    initializeUserPosts(user);
                    sendEmail(user, "Webvnue Registration", string.Format("Dear, {0} <br/><br/> Thank you for joining Webvnue. <br/><br/> You're on your way to becoming your own boss. <br/><br/> Best Regards, <br/>Team Webvnue", user.FirstName));

                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                    ClaimsIdentity identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthenticationProperties props = new AuthenticationProperties();

                    authenticationManager.SignIn(props, identity);

                    return Redirect(Url.Content(string.Format("~/{0}", user.UserName)));

                }
                else
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                    ModelState.AddModelError("Email", "Email already exists");
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

        [Route("{user}")]
        public ActionResult Personal(string user)
        {
            Models.MyIdentityUser userLoggedIn = getCurrentUser();

            if (userLoggedIn != null)
            {
                ViewData["CurrentUser"] = userLoggedIn;
            }

            Models.MyIdentityUser requestedUserPage = findUser(user);

            if (requestedUserPage != null)
            {
                ViewData["VisitedUser"] = requestedUserPage;
                ViewData["VisitedUserImages"] = getUserImageIdList(requestedUserPage);
                ViewData["VisitedUserImagesCount"] = getUserImageIdList(requestedUserPage).Count;
                ViewData["VisitedUserReferralListCount"] = getReferralList(requestedUserPage).Count;
                ViewData["VisitedUserProfileBio"] = getUserProfileBio(requestedUserPage.Id);
                ViewData["FollowingCount"] = getUserFollowingCount(requestedUserPage.Id);
                ViewData["FollowerCount"] = getUserFollowerCount(requestedUserPage.Id);

                ViewData["CurrentUserIsFollowing"] = (userLoggedIn != null) ? checkIfFollowing(userLoggedIn.Id, requestedUserPage.Id) : false;

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

            if (db.UserProfileImages.Find(getCurrentUser().Id) != null)
            {
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

                    var post = new Models.Post() {
                        Id = Guid.NewGuid().ToString(),
                        UserId = getCurrentUser().Id,
                        ImageData = imageData,
                        TimeStamp = DateTime.Now
                    };

                    foreach(var follower in getFollowers(getCurrentUser().Id))
                    {
                        db.UserPosts.Find(follower.Id).Posts.Add(post);
                    }

                    db.UserImages.Add(userImage);
                    db.SaveChanges();
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult ajaxUserProfileBio(string id)
        {

            var db = new Models.MyIdentityDbContext();

            Models.UserProfileBio bio = new Models.UserProfileBio();

            foreach (var obj in db.UserProfileBio)
            {
                if (obj.UserID == id)
                {
                    bio = obj;
                }
            }

            return Json(new
            {
                Bio = bio
            });
        }

        [HttpPost]
        public ActionResult saveBio(string id, string AboutMe, string Location, string Gender, string Quote)
        {
            updateUserBio(id, AboutMe, Location, Gender, Quote);

            return null;
        }

        [HttpPost]
        public ActionResult addFollowing(string id)
        {
            addNewFollow(getCurrentUser().Id, id);

            return null;
        }

        [HttpPost]
        public ActionResult removeFollowing(string id)
        {
            removeFollow(getCurrentUser().Id, id);

            return null;
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
        }

        public ActionResult photo(string id)
        {
            ViewData["ImageId"] = id;
            return View();
        }

        public ActionResult deletephoto(string id)
        {
            var db = new Models.MyIdentityDbContext();

            foreach (var obj in db.UserImages)
            {
                if (obj.Id == id)
                {
                    db.UserImages.Remove(obj);
                }
            }

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        private Models.UserProfileBio getUserProfileBio(string id)
        {
            var db = new Models.MyIdentityDbContext();

            Models.UserProfileBio bio = new Models.UserProfileBio();

            foreach (var obj in db.UserProfileBio)
            {
                if (obj.UserID == id)
                {
                    bio = obj;
                }
            }

            return bio;

            /*
            return Json(new
            {
                Bio = bio
            });
            */
        }

        private void updateUserBio(string id, string AboutMe, string Location, string Gender, string Quote)
        {
            var db = new Models.MyIdentityDbContext();

            foreach (var obj in db.UserProfileBio)
            {
                if (obj.UserID == id)
                {
                    obj.AboutMe = AboutMe;
                    obj.Location = Location;
                    obj.Gender = Gender;
                    obj.Quote = Quote;
                }
            }

            db.SaveChanges();
        }

        private List<string> getUserImageIdList(Models.MyIdentityUser user)
        {
            var db = new Models.MyIdentityDbContext();

            List<string> userImageIdList = new List<string>();

            foreach (var obj in db.UserImages)
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

        private bool validateToken(string Token)
        {
            Models.MyIdentityUser user = userManager.FindById(Token);

            if (user != null)
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

        private void addNewFollow(string userId, string followingUserId)
        {
            var db = new Models.MyIdentityDbContext();

            Models.Follows newFollower = new Models.Follows();
            newFollower.Id = Guid.NewGuid().ToString();
            newFollower.UserId = userId;
            newFollower.FollowingUserId = followingUserId;

            db.UserFollowers.Add(newFollower);
            db.SaveChanges();
        }

        private void removeFollow(string currentUserId, string followingUserId)
        {
            var db = new Models.MyIdentityDbContext();

            foreach (var obj in db.UserFollowers)
            {
                if (obj.UserId == currentUserId && obj.FollowingUserId == followingUserId)
                {
                    db.UserFollowers.Remove(obj);
                }
            }

            db.SaveChanges();
        }

        private void addUserDefaultProfileBio(Models.MyIdentityUser user)
        {
            var db = new Models.MyIdentityDbContext();

            Models.UserProfileBio bio = new Models.UserProfileBio();

            bio.Id = Guid.NewGuid().ToString();
            bio.UserID = user.Id;
            bio.AboutMe = string.Format("Hello World!, I'm {0}. Let's make some money!", user.FirstName);
            bio.Location = "Webvnue City";
            bio.Gender = "Human";
            bio.Quote = "\"You only live once, but if you do it right, once is enough.\" ― Mae West";

            db.UserProfileBio.Add(bio);
            db.SaveChanges();
        }

        private void sendEmail(Models.MyIdentityUser user, string subject, string body)
        {
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("webvnue@gmail.com", "Webvnue"), new System.Net.Mail.MailAddress(user.Email));
            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("webvnue@gmail.com", "Password999");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        private void fillMissingPics()
        {
            var db = new Models.MyIdentityDbContext();

            Models.UserProfileImage defaultImg = db.UserProfileImages.Find("164eb47a-ac68-4e92-8baa-56dd5f730a80");

            foreach (var user in db.Users)
            {
                if (db.UserProfileImages.Find(user.Id) == null)
                {
                    db.UserProfileImages.Add(new Models.UserProfileImage()
                    {
                        UserId = user.Id,
                        ImageData = defaultImg.ImageData,
                        FileName = "Default Image"
                    });
                }
            }

            db.SaveChanges();
        }

        private void addDefaultPicture(Models.MyIdentityUser user)
        {
            var db = new Models.MyIdentityDbContext();
            Models.UserProfileImage defaultImg = db.UserProfileImages.Find("164eb47a-ac68-4e92-8baa-56dd5f730a80");
            db.UserProfileImages.Add(new Models.UserProfileImage()
            {
                UserId = user.Id,
                ImageData = defaultImg.ImageData,
                FileName = "Default Image"
            });
            db.SaveChanges();
        }

        private void initializeUserPosts(Models.MyIdentityUser user)
        {
            var db = new Models.MyIdentityDbContext();
            db.UserPosts.Add(new Models.UserPosts()
            {
                UserId = user.Id,
                Posts = new List<Models.Post>()
            });

            db.SaveChanges();
        }

        private bool checkIfFollowing(string currentUserId, string visitedUserId)
        {
            var db = new Models.MyIdentityDbContext();
            
            foreach(var obj in db.UserFollowers)
            {
                if(obj.UserId == currentUserId && obj.FollowingUserId == visitedUserId)
                {
                    return true;
                }
            }

            return false;
        }

        private int getUserFollowingCount(string userId)
        {
            var db = new Models.MyIdentityDbContext();

            int count = 0;

            foreach(var obj in db.UserFollowers)
            {
                if(obj.UserId == userId)
                {
                    count++;
                }
            }

            return count;
        }

        private int getUserFollowerCount(string userId)
        {
            var db = new Models.MyIdentityDbContext();

            int count = 0;

            foreach (var obj in db.UserFollowers)
            {
                if (obj.FollowingUserId == userId)
                {
                    count++;
                }
            }

            return count;
        }

        private List<Models.MyIdentityUser> getFollowers(string userId)
        {
            var db = new Models.MyIdentityDbContext();

            List<Models.MyIdentityUser> followerList = new List<Models.MyIdentityUser>();

            foreach (var obj in db.UserFollowers)
            {
                if (obj.FollowingUserId == userId)
                {
                    followerList.Add(userManager.FindById(obj.UserId));
                }
            }

            return followerList;
        }
    }
}

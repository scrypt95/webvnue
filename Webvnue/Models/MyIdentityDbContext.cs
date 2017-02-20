using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class MyIdentityDbContext : IdentityDbContext<MyIdentityUser>
    {
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<UserCreditCard> UserCCInfo { get; set; }
        public DbSet<UserProfileImage> UserProfileImages { get; set; }
        public DbSet<UserImage> UserImages { get; set; }

        public MyIdentityDbContext() : base("webvnue")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyIdentityDbContext>());
        }




    }
}
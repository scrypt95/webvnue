using Webvnue.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Webvnue
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MyIdentityDbContext db = new MyIdentityDbContext();
            RoleStore<MyIdentityRole> roleStore = new RoleStore<MyIdentityRole>(db);
            RoleManager<MyIdentityRole> roleManager = new RoleManager<MyIdentityRole>(roleStore);

            if (!roleManager.RoleExists("Administrator"))
            {
                MyIdentityRole newRole = new MyIdentityRole("Administrator", "Administrators can add, edit and delete data.");
                roleManager.Create(newRole);
            }

            if (!roleManager.RoleExists("User"))
            {
                MyIdentityRole newRole = new MyIdentityRole("User", "Regular users.");
                roleManager.Create(newRole);
            }
        }
    }
}

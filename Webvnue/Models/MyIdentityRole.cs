using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class MyIdentityRole : IdentityRole
    {
        public string Description { get; set; }

        public MyIdentityRole()
        {

        }

        public MyIdentityRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }
    }
}
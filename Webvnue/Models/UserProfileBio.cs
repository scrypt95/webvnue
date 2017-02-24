using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserProfileBio
    {
        [Key]
        public string Id { get; set; }
        public string UserID { get; set; }
        public string AboutMe { get; set; }
        public string Location { get; set; }
        public string Gender { get; set; }
        public string Quote { get; set; }

    }
}
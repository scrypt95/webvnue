using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserPosts
    {
        [Key]
        public string UserId { get; set; }
        public List<Models.Post> Posts { get; set; }
    }
}
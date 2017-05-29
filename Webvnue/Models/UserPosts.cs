using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserPosts
    {
        [Key]
        public string UserId { get; set; }
        public virtual List<Models.Post> Posts { get; set; }

        public UserPosts()
        {
            Posts = new List<Post>();
        }
    }
}
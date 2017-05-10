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
        public virtual Collection<Models.Post> Posts { get; set; }

        public UserPosts()
        {
            Posts = new Collection<Post>();
        }
    }
}
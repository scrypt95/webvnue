using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OriginalPostUserId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual Collection<Comment> Comments { get; set; }
        [NotMapped]
        public Models.MyIdentityUser OriginalPostUser { get; set; }

        public Post()
        {
            Comments = new Collection<Comment>();
        }
    }
}
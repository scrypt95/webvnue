using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class Post
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
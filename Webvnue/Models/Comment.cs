using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string OriginalUserId { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        [NotMapped]
        public Models.MyIdentityUser OriginalUser { get; set; }
    }
}
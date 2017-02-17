using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserProfileImage
    {
        [Key]
        public string UserId { get; set; }
        public Byte[] ImageData { get; set; }
        public string FileName { get; set; }
    }
}
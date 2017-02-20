using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserImage
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public double Rating { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Views { get; set; }
    }
}
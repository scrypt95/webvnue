using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class Follows
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FollowingUserId { get; set; }
    }
}
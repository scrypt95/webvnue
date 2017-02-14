using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class UserCreditCard
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CardId { get; set; }
        public DateTime timestamp { get; set; }
    }
}
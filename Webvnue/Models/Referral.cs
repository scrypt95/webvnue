using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{

    public class Referral
    {
        public string Id { get; set; }
        public string ReferrerId { get; set; }
        public string RefereeId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class forgotpassword
    {
        [Required(ErrorMessage = "You must enter a email!")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
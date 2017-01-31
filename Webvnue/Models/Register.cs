using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter a first name.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter a last name.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter a username.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter a Email address.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter a valid date of birth (Ex: 1/1/2000).")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Please enter a password.")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Please confirm your password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
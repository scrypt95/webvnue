using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class CreditCardModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public string cvv2 { get; set; }
        public string CardType { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

    }
}
using log4net.Repository.Hierarchy;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class transactionController : Controller
    {
        // GET: transaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult subscription()
        {
            return View();
        }

        public ActionResult success()
        {
            return View();
        }

        public ActionResult failed()
        {
            return View();
        }

        public ActionResult error()
        {
            return View();
        }

        public ActionResult PaymentWithCreditCard()
        {
            try
            {
                // ### Api Context
                // Pass in a `APIContext` object to authenticate 
                // the call and to send a unique request id 
                // (that ensures idempotency). The SDK generates
                // a request id if you do not pass one explicitly. 
                // See [Configuration.cs](/Source/Configuration.html) to know more about APIContext.
                var apiContext = Models.Configuration.GetAPIContext();

                // A transaction defines the contract of a payment - what is the payment for and who is fulfilling it. 
                var transaction = new Transaction()
                {
                    amount = new Amount()
                    {
                        currency = "USD",
                        total = "7"
                        /*
                        details = new Details()
                        {
                            shipping = "1",
                            subtotal = "5",
                            tax = "1"
                        }*/
                    },
                    description = "Webvnue Monthly Subscription",
                    item_list = new ItemList()
                    {
                        items = new List<Item>()
                    {
                        new Item()
                        {
                            name = "Webvnue Subscription",
                            currency = "USD",
                            price = "7",
                            quantity = "1",
                            //sku = "sku"
                        }
                    }
                        /*
                        shipping_address = new ShippingAddress
                        {
                            city = "San Jose",
                            country_code = "US",
                            line1 = "San Jose",
                            postal_code = "95131",
                            state = "CA",
                            recipient_name = "Peter North"
                        }*/
                    },
                    //invoice_number = new Random().Next(999999).ToString()
                };

                // A resource representing a Payer that funds a payment.
                var payer = new Payer()
                {
                    payment_method = "credit_card",
                    funding_instruments = new List<FundingInstrument>()
                {
                    new FundingInstrument()
                    {
                        credit_card = new CreditCard()
                        {
                            billing_address = new Address()
                            {
                                city = "San Jose",
                                country_code = "US",
                                line1 = "1 Main St",
                                postal_code = "95131",
                                state = "CA"
                            },
                            cvv2 = "874",
                            expire_month = 2,
                            expire_year = 2022,
                            first_name = "Peter",
                            last_name = "North",
                            number = "4032039435922259",
                            type = "visa"
                        }
                    }
                },
                    payer_info = new PayerInfo
                    {
                        email = "philmikehuntplz@email.com"
                    }
                };

                // A Payment resource; create one using the above types and intent as `sale` or `authorize`
                var payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = new List<Transaction>() { transaction }
                };

                var createdPayment = payment.Create(apiContext);
            }
            catch(Exception e)
            {
                return View("error");
            }
            return View("success");
        }
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webvnue.Controllers
{
    public class servicesController : Controller
    {
        private UserManager<Models.MyIdentityUser> userManager;

        public servicesController()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();

            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            userManager = new UserManager<Models.MyIdentityUser>(userStore);
        }

        public ActionResult subscription()
        {
            //comment
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult subscribe()
        {

            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

            if (user.Subscription)
            {
                return RedirectToAction("Index", "Home");
            }

            var list = new SelectList(CountryList(), "Key", "Value");
            var sortList = list.OrderBy(p => p.Text).ToList();
            sortList.Insert(0, new SelectListItem()
            {
                Text = "United States",
                Value = "US",
            });
            ViewBag.Countries = sortList;
            ViewBag.ExpireMonth = getExpireMonth();
            ViewBag.ExpireYear = getExpireYear();

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult subscribe(Models.CreditCardModel model)
        {
            //bool result = processPayment(model, getCurrentUser());
            bool result = createBillingAgreement(model, getCurrentUser());

            if (result == true)
            {
                return View("success");
            }
            else
            {
                return View("failed");
            }

        }

        [HttpPost]
        public ActionResult unsubscribe()
        {

            Models.MyIdentityUser user = userManager.FindById(getCurrentUser().Id);
            user.Subscription = false;
            IdentityResult result = userManager.Update(user);

            return null;
        }

        [Authorize]
        public ActionResult success()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

            if (user.Subscription)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [Authorize]
        public ActionResult failed()
        {
            Models.MyIdentityUser user = getCurrentUser();

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }

            if (user.Subscription)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public List<SelectListItem> getExpireMonth()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            for (int x = 1; x < 13; x++)
            {
                list.Add(new SelectListItem()
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                });
            }

            return list;
        }

        public List<SelectListItem> getExpireYear()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            for (int x = 0; x < 26; x++)
            {
                list.Add(new SelectListItem()
                {
                    Text = (DateTime.Now.Year + x).ToString(),
                    Value = (DateTime.Now.Year + x).ToString()
                });
            }

            return list;
        }

        public Dictionary<string, string> CountryList()
        {
            //Creating Dictionary
            Dictionary<string, string> cultureList = new Dictionary<string, string>();

            //getting the specific CultureInfo from CultureInfo class
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo getCulture in getCultureInfo)
            {
                //creating the object of RegionInfo class
                RegionInfo getRegionInfo = new RegionInfo(getCulture.LCID);
                //adding each country Name into the Dictionary
                if (!(cultureList.ContainsKey(getRegionInfo.Name)))
                {
                    cultureList.Add(getRegionInfo.Name, getRegionInfo.EnglishName);
                }
            }
            //returning country list
            return cultureList;
        }

        private Models.MyIdentityUser getCurrentUser()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();
            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            UserManager<Models.MyIdentityUser> userManager = new UserManager<Models.MyIdentityUser>(userStore);

            Models.MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

            return user;
        }

        private UserManager<Models.MyIdentityUser> getUserManager()
        {
            Models.MyIdentityDbContext db = new Models.MyIdentityDbContext();
            UserStore<Models.MyIdentityUser> userStore = new UserStore<Models.MyIdentityUser>(db);
            UserManager<Models.MyIdentityUser> userManager = new UserManager<Models.MyIdentityUser>(userStore);

            return userManager;
        }

        public bool processPayment(Models.CreditCardModel model, Models.MyIdentityUser user)
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
                        }
                    }
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
                                city = model.City,
                                country_code = model.Country,
                                line1 = model.Address,
                                postal_code = model.ZipCode,
                                state = model.State
                            },
                            cvv2 = model.cvv2,
                            expire_month = Int32.Parse(model.ExpireMonth),
                            expire_year = Int32.Parse(model.ExpireYear),
                            first_name = model.FirstName,
                            last_name = model.LastName,
                            number = model.CardNumber,
                            type = model.CardType
                        }
                    }
                },
                    payer_info = new PayerInfo
                    {
                        email = user.Email
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
            catch (Exception e)
            {
                return false;
            }

            Models.MyIdentityUser subscribedUser = userManager.FindById(user.Id);
            subscribedUser.Subscription = true;
            IdentityResult result = userManager.Update(subscribedUser);

            return true;
        }

        public bool createBillingAgreement(Models.CreditCardModel model, Models.MyIdentityUser user)
        {
            var apiContext = Models.Configuration.GetAPIContext();

            var billingPlan = new Models.BillingPlanCreate();

            var plan = billingPlan.CreatePlanObject(System.Web.HttpContext.Current);
            var guid = Convert.ToString((new Random()).Next(100000));
            plan.merchant_preferences.return_url = Request.Url.ToString() + "?guid=" + guid;
            plan.merchant_preferences.cancel_url = Request.Url.ToString();

            var createdPlan = plan.Create(apiContext);

            var patchRequest = new PatchRequest()
                {
                    new Patch()
                    {
                        op = "replace",
                        path = "/",
                        value = new Plan() { state = "ACTIVE" }
                    }
                };

            createdPlan.Update(apiContext, patchRequest);

            var payer = new Payer
            {
                payment_method = "credit_card",
                funding_instruments = new List<FundingInstrument>
                {
                    new FundingInstrument
                    {
                        credit_card = new CreditCard
                        {
                            billing_address = new Address
                            {
                                city = model.City,
                                country_code = model.Country,
                                line1 = model.Address,
                                postal_code = model.ZipCode,
                                state = model.State
                            },
                            cvv2 = model.cvv2,
                            expire_month = Int32.Parse(model.ExpireMonth),
                            expire_year = Int32.Parse(model.ExpireYear),
                            first_name = model.FirstName,
                            last_name = model.LastName,
                            number = model.CardNumber,
                            type = model.CardType
                        }
                    }
                }
            };

            var shippingAddress = new ShippingAddress()
            {
                line1 = "111 First Street",
                city = "Saratoga",
                state = "CA",
                postal_code = "95070",
                country_code = "US"
            };

            var agreement = new Agreement()
            {
                name = "Webvnue",
                description = "Monthly Subscription",
                //start_date = "2015-02-19T00:37:04Z",
                start_date = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                payer = payer,
                plan = new Plan() { id = createdPlan.id },
                shipping_address = shippingAddress
            };

            var createdAgreement = agreement.Create(apiContext);

            return true;
        }


        public void storeCreditCardInVault()
        {

        }
    }
}
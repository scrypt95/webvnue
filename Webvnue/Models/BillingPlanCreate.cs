using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webvnue.Models
{
    public class BillingPlanCreate
    {
        private Currency GetCurrency(string value)
        {
            return new Currency() { value = value, currency = "USD" };
        }

        public Plan CreatePlanObject(HttpContext httpContext)
        {
            var shippingChargeModel = new ChargeModel()
            {
                type = "SHIPPING",
                amount = GetCurrency("9.99")
            };

            return new Plan
            {
                name = "Webvnue",
                description = "Monthly Subscription",
                type = "INFINITE",
                merchant_preferences = new MerchantPreferences()
                {
                    setup_fee = GetCurrency("0"),
                    return_url = httpContext.Request.Url.ToString(),
                    cancel_url = httpContext.Request.Url.ToString() + "?cancel",
                    auto_bill_amount = "YES",
                    initial_fail_amount_action = "CONTINUE",
                    max_fail_attempts = "0"
                },
                payment_definitions = new List<PaymentDefinition>
                {
                  new PaymentDefinition
                  {
                      name = "Monthly Subscription",
                      type = "REGULAR",
                      frequency = "MONTH",
                      frequency_interval = "1",
                      amount = GetCurrency("7.00"),
                      cycles = "0",
                      charge_models = new List<ChargeModel>
                      {
                            new ChargeModel
                            {
                                type = "TAX",
                                amount = GetCurrency("2.47")
                            },
                            shippingChargeModel
                      }
                  }
    }
};
        }
    }
}
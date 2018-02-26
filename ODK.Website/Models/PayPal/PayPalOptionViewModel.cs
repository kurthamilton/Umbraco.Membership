using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Models.PayPal
{
    public class PayPalOptionViewModel
    {
        public PayPalOptionViewModel(IPublishedContent content, string currencySymbol)
        {
            CurrencySymbol = currencySymbol;

            Amount = content.GetPropertyValue<double>("payPalOptionAmount");
            Description = content.GetPropertyValue<string>("payPalOptionDescription");
            MaxItems = Math.Min(content.GetPropertyValue<int>("payPalOptionMaxItems"), 1);
            Name = content.GetPropertyValue<string>("payPalOptionName");
            Title = content.GetPropertyValue<string>("payPalOptionTitle");
        }

        public double Amount { get; }

        public string CurrencySymbol { get; }

        public string Description { get; }

        public int MaxItems { get; }

        public string Name { get; }

        public string Title { get; }
    }
}
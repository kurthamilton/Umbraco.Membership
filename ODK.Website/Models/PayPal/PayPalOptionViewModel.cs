using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Models.PayPal
{
    public class PayPalOptionViewModel
    {
        public PayPalOptionViewModel()
        {
        }

        public PayPalOptionViewModel(IPublishedContent content)
        {
            Amount = content.GetPropertyValue<double>("payPalOptionAmount");
            Description = content.GetPropertyValue<string>("payPalOptionDescription");
            Id = content.Id;
            MaxItems = Math.Max(content.GetPropertyValue<int>("payPalOptionMaxItems"), 1);
            Title = content.GetPropertyValue<string>("payPalOptionTitle");
        }

        public double Amount { get; }

        public string Description { get; }

        public int Id { get; set; }

        public int MaxItems { get; }

        public int Quantity { get; set; }

        public string Title { get; }

        public string GetFormattedAmount(string currencySymbol)
        {
            return string.Format("{0}{1:0.00}", currencySymbol, Amount);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Models.PayPal
{
    public class PayPalButtonViewModel
    {
        public static Dictionary<string, string> CurrencySymbols = new Dictionary<string, string>
        {
            { "AUD", "$" },
            { "EUR", "€" },
            { "GBP", "£" },
            { "USD", "$" }
        };

        public PayPalButtonViewModel()
        {
        }

        public PayPalButtonViewModel(IPublishedContent chapter, IEnumerable<IPublishedContent> options)
            : this(chapter)
        {
            Options = options.Select(x => new PayPalOptionViewModel(x)).ToArray();
        }

        public PayPalButtonViewModel(IPublishedContent chapter, IPublishedContent option)
            : this(chapter)
        {
            Options = new[]
            {
                new PayPalOptionViewModel(option)
            };
        }

        private PayPalButtonViewModel(IPublishedContent chapter)
        {
            CurrencyCode = chapter.GetPropertyValue<string>("payPalCurrencyCode");
            CurrencySymbol = CurrencySymbols[CurrencyCode];
        }

        public string CurrencyCode { get; }

        public string CurrencySymbol { get; }

        public PayPalOptionViewModel[] Options { get; set; }
    }
}
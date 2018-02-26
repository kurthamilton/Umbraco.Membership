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

        public PayPalButtonViewModel(string buttonId, IPublishedContent chapter, IEnumerable<IPublishedContent> options)
            : this(buttonId, chapter)
        {
            Options = options.Select(x => new PayPalOptionViewModel(x, CurrencySymbol)).ToArray();
        }

        public PayPalButtonViewModel(string buttonId, IPublishedContent chapter, IPublishedContent option)
            : this(buttonId, chapter)
        {
            Options = new[]
            {
                new PayPalOptionViewModel(option, CurrencySymbol)
            };
        }

        private PayPalButtonViewModel(string buttonId, IPublishedContent chapter)
        {
            ButtonId = buttonId;
            CurrencyCode = chapter.GetPropertyValue<string>("payPalCurrencyCode");
            CurrencySymbol = CurrencySymbols[CurrencyCode];
        }

        public string ButtonId { get; }

        public string CurrencyCode { get; }

        public string CurrencySymbol { get; }

        public IReadOnlyCollection<PayPalOptionViewModel> Options { get; }
    }
}
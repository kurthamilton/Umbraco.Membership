using System.Web.Mvc;

namespace ODK.Website.Models
{
    public class EmailViewModel
    {
        [AllowHtml]
        public string Body { get; set; }

        public string Subject { get; set; }
    }
}
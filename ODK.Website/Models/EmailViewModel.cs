using System.Web.Mvc;

namespace ODK.Website.Models
{
    public class EmailViewModel
    {
        public EmailViewModel(string id)
        {
            Id = id;
        }

        [AllowHtml]
        public string Body { get; set; }

        public string Id { get; }

        public string Subject { get; set; }
    }
}
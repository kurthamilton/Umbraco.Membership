using System.Web;
using System.Web.Security;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Security;
using Umbraco.Web;

namespace ODK.Umbraco.Security
{
    public static class SecurityHelper
    {
        public static IUser CurrentAdminUser()
        {
            FormsAuthenticationTicket auth = new HttpContextWrapper(HttpContext.Current).GetUmbracoAuthTicket();
            return auth == null ? null : UmbracoContext.Current.Application.Services.UserService.GetByUsername(auth.Name);
        }
    }
}

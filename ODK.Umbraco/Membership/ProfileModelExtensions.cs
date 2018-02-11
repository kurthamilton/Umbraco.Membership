using System;
using System.Linq;
using Umbraco.Web.Models;

namespace ODK.Umbraco.Membership
{
    public static class ProfileModelExtensions
    {
        public static UmbracoProperty GetProperty(this ProfileModel profile, string name)
        {
            if (profile == null)
            {
                return null;
            }

            return profile.MemberProperties.FirstOrDefault(x => x.Alias.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void SetProperty(this ProfileModel profile, string name, object value)
        {
            UmbracoProperty property = profile.GetProperty(name);
            if (property == null && value == null)
            {
                return;                
            }

            if (property != null && value == null)
            {
                profile.MemberProperties.Remove(property);
            }
            else if(property == null)
            {
                property = new UmbracoProperty
                {
                    Alias = MemberPropertyNames.ChapterId,
                    Name = MemberPropertyNames.ChapterId,
                    Value = value.ToString()
                };

                profile.MemberProperties.Add(property);
            }
            else
            {
                property.Value = value.ToString();
            }
        }
    }
}

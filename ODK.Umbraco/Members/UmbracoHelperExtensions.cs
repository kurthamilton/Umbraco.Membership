using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public static class UmbracoHelperExtensions
    {
        private const int KnittingExperienceOptionsDataTypeId = 1256;

        public static IEnumerable<string> GetKnittingExperienceOptions(this UmbracoHelper helper)
        {
            return helper.DataTypeService.GetPreValuesCollectionByDataTypeId(KnittingExperienceOptionsDataTypeId)
                                         .PreValuesAsDictionary
                                         .Select(x => x.Value.Value);
        }
    }
}

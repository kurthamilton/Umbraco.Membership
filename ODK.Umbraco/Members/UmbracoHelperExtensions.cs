using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public static class UmbracoHelperExtensions
    {
        private const int KnittingExperienceOptionsDataTypeId = 1256;

        public static IDictionary<int, string> GetKnittingExperienceOptions(this UmbracoHelper helper)
        {
            return helper.DataTypeService.GetPreValuesCollectionByDataTypeId(KnittingExperienceOptionsDataTypeId)
                                         .PreValuesAsDictionary
                                         .ToDictionary(x => x.Value.Id, x => x.Value.Value);
        }
    }
}

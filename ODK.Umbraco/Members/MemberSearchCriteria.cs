using System;
using System.Collections.Generic;

namespace ODK.Umbraco.Members
{
    public class MemberSearchCriteria
    {
        public MemberSearchCriteria(int chapterId)
        {
            ChapterId = chapterId;
        }

        public int ChapterId { get; }

        public int? MaxItems { get; set; }

        public bool ShowAll { get; set; }

        public Func<IEnumerable<MemberModel>, IEnumerable<MemberModel>> Sort { get; set; }

        public IReadOnlyCollection<MemberTypes> Types { get; set; }
}
}

namespace ODK.Umbraco.Events
{
    public class EventSearchCriteria
    {
        public bool? FutureOnly { get; set; }

        public int? Month { get; set; }

        public int? PageSize { get; set; }
    }
}

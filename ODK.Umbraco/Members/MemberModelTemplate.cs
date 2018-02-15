namespace ODK.Umbraco.Members
{
    public class MemberModelTemplate<T> where T : MemberModel
    {
        public MemberModelTemplate(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}

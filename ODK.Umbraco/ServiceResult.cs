using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ODK.Umbraco
{
    public class ServiceResult
    {
        public ServiceResult(string propertyName, string message)
        {
            Messages = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { propertyName, message }
            });
        }

        public ServiceResult(IDictionary<string, string> messages)
        {
            Messages = new ReadOnlyDictionary<string, string>(messages);
        }

        public ServiceResult(bool success)
        {
            Success = success;
        }

        public IReadOnlyDictionary<string, string> Messages { get; private set; } = new Dictionary<string, string>();

        public bool Success { get; private set; }
    }
}

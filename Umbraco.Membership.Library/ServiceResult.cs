namespace ODK.Umbraco
{
    public class ServiceResult
    {
        public ServiceResult(string message)
        {
            Message = message;
        }

        public ServiceResult(bool success)
        {
            Success = success;
        }

        public string Message { get; private set; }

        public bool Success { get; private set; }
    }
}

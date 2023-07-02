namespace AuthenticationAndAuthorizationInfrastructure.Result
{
    public class Error
    {
        private Error(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static Error Create(string message) => new(message);
    }
}

using AuthenticationAndAuthorizationInfrastructure.Result;
using OneOf;

namespace AuthenticationAndAuthorizationInfrastructure
{
    public static class AuthCookieService
    {
        private const string AuthCookieKey = "auth-cookie";

        public static KeyValuePair<string, string> CreateAuthCookie(IEnumerable<KeyValuePair<string, string>> claims, Func<string, string> protector)
        {
            string claimsString = string.Join(";", claims.Select(authInfo => $"{authInfo.Key}:{authInfo.Value}"));

            return new KeyValuePair<string, string>(AuthCookieKey, protector($"auth={claimsString}"));
        }

        public static void RemoveAuthCookie(Action<string> remove)
        {
            remove(AuthCookieKey);
        }

        public static OneOf<KeyValuePair<string, string>, Error> GetKeyValueFromAuthCookieByKey(IEnumerable<KeyValuePair<string, string>>? cookies, string key, Func<string, string> unProtector)
        {
            if (cookies == null || !cookies.Any(c => c.Key == AuthCookieKey))
                return Error.Create("Auth Cookie Not Found");

            string cookieValue = cookies.FirstOrDefault(c => c.Key == AuthCookieKey).Value;

            if (string.IsNullOrEmpty(cookieValue))
                return Error.Create("Auth Cookie Not Found");

            var value = GetCookieValueByKey(unProtector(cookieValue), key);

            if (string.IsNullOrEmpty(value))
                return Error.Create($"{key} not found in auth cookie");

            KeyValuePair<string, string> keyValue = new(key, value);
            return keyValue;
        }

        private static string? GetCookieValueByKey(string cookieValue, string key)
        {
            var tokens = cookieValue.Split('=');
            var keyValuePairs = tokens.ElementAtOrDefault(1)?.Split(';');

            if (keyValuePairs == null)
                return null;

            var token = keyValuePairs.FirstOrDefault(token => token.Contains(key));
            var parts = token?.Split(':');

            return parts?.ElementAtOrDefault(1);
        }
    }
}

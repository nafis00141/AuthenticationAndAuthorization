using AuthenticationAndAuthorizationInfrastructure;

namespace AuthenticationAndAuthorizationInfrastructureSpecs
{
    [TestFixture]
    public class AuthCookieServiceTests
    {
        [Test]
        public void CreateAuthCookie_ReturnsKeyValuePairWithCorrectAuthCookieKey()
        {
            // Arrange
            var claims = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("claim1", "value1"),
                new KeyValuePair<string, string>("claim2", "value2")
            };

            // Act
            var result = AuthCookieService.CreateAuthCookie(claims, (value => value));

            // Assert
            Assert.That(result.Key, Is.EqualTo("auth-cookie"));
        }

        [Test]
        public void CreateAuthCookie_ReturnsKeyValuePairWithCorrectAuthCookieValue()
        {
            // Arrange
            var claims = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("claim1", "value1"),
                new KeyValuePair<string, string>("claim2", "value2")
            };

            // Act
            var result = AuthCookieService.CreateAuthCookie(claims, (value => value));

            // Assert
            Assert.That(result.Value, Is.EqualTo("auth=claim1:value1;claim2:value2"));
        }

        [Test]
        public void RemoveAuthCookie_CallsRemoveActionWithCorrectAuthCookieKey()
        {
            // Arrange
            string removedKey = "";
            void Remove(string key) => removedKey = key;

            // Act
            AuthCookieService.RemoveAuthCookie(Remove);

            // Assert
            Assert.That(removedKey, Is.EqualTo("auth-cookie"));
        }

        [Test]
        public void GetKeyValueFromAuthCookieByKey_ReturnsErrorWhenAuthCookieNotPresent()
        {
            // Arrange
            var cookies = new List<KeyValuePair<string, string>>();

            // Act
            var result = AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, "key", (value => value));

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("Auth Cookie Not Found"));
        }

        [Test]
        public void GetKeyValueFromAuthCookieByKey_ReturnsErrorWhenAuthCookieValueIsNull()
        {
            // Arrange
            var cookies = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("auth-cookie", "")
            };

            // Act
            var result = AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, "key", (value => value));

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("Auth Cookie Not Found"));
        }

        [Test]
        public void GetKeyValueFromAuthCookieByKey_ReturnsErrorWhenKeyNotFoundInAuthCookie()
        {
            // Arrange
            var cookies = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("auth-cookie", "auth=key1:value1;key2:value2")
            };

            // Act
            var result = AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, "key3", (value => value));

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("key3 not found in auth cookie"));
        }

        [Test]
        public void GetKeyValueFromAuthCookieByKey_ReturnsKeyValuePairWithCorrectKeyAndValue()
        {
            // Arrange
            var cookies = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("auth-cookie", "auth=key1:value1;key2:value2")
            };

            // Act
            var result = AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, "key2", (value => value));

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0.Key, Is.EqualTo("key2"));
            Assert.That(result.AsT0.Value, Is.EqualTo("value2"));
        }
    }
}
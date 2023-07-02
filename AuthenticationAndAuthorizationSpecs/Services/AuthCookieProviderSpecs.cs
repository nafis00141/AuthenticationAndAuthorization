using AuthenticationAndAuthorization.Services;
using FakeItEasy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthenticationAndAuthorizationSpecs.Services
{
    [TestFixture]
    public class AuthCookieProviderSpecs
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IDataProtectionProvider _dataProtectionProvider;
        private HttpContext _httpContext;
        private IResponseCookies _responseCookies;
        private IRequestCookieCollection _requestCookies;
        private IDataProtector _dataProtector;

        private AuthCookieProvider _authCookieProvider;

        [SetUp]
        public void Setup()
        {
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();
            _dataProtectionProvider = A.Fake<IDataProtectionProvider>();
            _httpContext = A.Fake<HttpContext>();
            _responseCookies = A.Fake<IResponseCookies>();
            _requestCookies = A.Fake<IRequestCookieCollection>();
            _dataProtector = A.Fake<IDataProtector>();

            A.CallTo(() => _httpContextAccessor.HttpContext).Returns(_httpContext);
            A.CallTo(() => _httpContext.Response.Cookies).Returns(_responseCookies);
            A.CallTo(() => _httpContext.Request.Cookies).Returns(_requestCookies);
            A.CallTo(() => _dataProtectionProvider.CreateProtector(A<string>._)).Returns(_dataProtector);

            _authCookieProvider = new AuthCookieProvider(_httpContextAccessor, _dataProtectionProvider);
        }

        [Test]
        public void AddAuthCookieFor_ShouldCreateAuthCookieAndAppendToResponseCookies()
        {
            // Arrange
            var user = new User(1, "Nafis", "nafis0014@gmail.com", UserRole.User, true);

            // Act
            _authCookieProvider.AddAuthCookieFor(user);

            // Assert
            A.CallTo(() => _responseCookies.Append(A<string>._, A<string>._, A<CookieOptions>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RemoveAuthCookie_ShouldCallRemoveAuthCookieMethod()
        {
            // Act
            _authCookieProvider.RemoveAuthCookie();

            // Assert
            A.CallTo(() => _responseCookies.Delete(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetUserIdFromAuthCookie_ShouldReturnUserIdFromAuthCookie()
        {
            // Arrange
            var cookies = new Dictionary<string, string>
            {
                { "auth-cookie", WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("auth=id:123;role:2")) }
            };
            A.CallTo(() => _requestCookies.GetEnumerator()).Returns(cookies.GetEnumerator());
            A.CallTo(() => _dataProtector.Unprotect(A<byte[]>._)).Returns(WebEncoders.Base64UrlDecode(cookies["auth-cookie"]));

            // Act
            var result = _authCookieProvider.GetUserIdFromAuthCookie();

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(123));
        }

        [Test]
        public void GetUserIdFromAuthCookie_ShouldReturnErrorWhenAuthCookieNotPresent()
        {
            // Arrange
            var cookies = new Dictionary<string, string>();
            A.CallTo(() => _requestCookies.GetEnumerator()).Returns(cookies.GetEnumerator());

            // Act
            var result = _authCookieProvider.GetUserIdFromAuthCookie();

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("Auth Cookie Not Found"));
        }

        [Test]
        public void GetUserRoleFromAuthCookie_ShouldReturnUserRoleFromAuthCookie()
        {
            // Arrange
            var cookies = new Dictionary<string, string>
            {
                { "auth-cookie", WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("auth=id:123;role:2")) }
            };
            A.CallTo(() => _requestCookies.GetEnumerator()).Returns(cookies.GetEnumerator());
            A.CallTo(() => _dataProtector.Unprotect(A<byte[]>._)).Returns(WebEncoders.Base64UrlDecode(cookies["auth-cookie"]));

            // Act
            var result = _authCookieProvider.GetUserRoleFromAuthCookie();

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(2));
        }

        [Test]
        public void GetUserRoleFromAuthCookie_ShouldReturnErrorWhenAuthCookieNotPresent()
        {
            // Arrange
            var cookies = new Dictionary<string, string>();
            A.CallTo(() => _requestCookies.GetEnumerator()).Returns(cookies.GetEnumerator());

            // Act
            var result = _authCookieProvider.GetUserRoleFromAuthCookie();

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("Auth Cookie Not Found"));
        }
    }

}

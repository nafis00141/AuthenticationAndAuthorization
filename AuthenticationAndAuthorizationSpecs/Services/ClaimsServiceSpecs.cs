using AuthenticationAndAuthorization.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthenticationAndAuthorizationSpecs.Services
{
    [TestFixture]
    public class ClaimsServiceSpecs
    {
        private ClaimsService _claimsService;
        private HttpContext _httpContext;

        [SetUp]
        public void Setup()
        {
            // Create a mock HttpContext with a fake ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim("id", "123"),
                new Claim("roleId", "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _httpContext = new DefaultHttpContext();
            _httpContext.User = principal;

            // Create an instance of ClaimsService with the mock HttpContext
            var httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext = _httpContext;

            _claimsService = new ClaimsService(httpContextAccessor);
        }

        [Test]
        public void Create_WithValidInput_ReturnsClaimsPrincipalWithClaims()
        {
            // Arrange
            long userId = 123;
            int userRoleId = 1;

            // Act
            var result = _claimsService.Create(userId, userRoleId);

            // Assert
            Assert.That(result?.Identity?.IsAuthenticated, Is.True);
            Assert.That(result?.Identity?.Name, Is.Null);
            Assert.That(result?.FindFirst("id")?.Value, Is.EqualTo(userId.ToString()));
            Assert.That(result?.FindFirst("roleId")?.Value, Is.EqualTo(userRoleId.ToString()));
        }

        [Test]
        public void GetUserIdFromClaim_WithValidClaim_ReturnsUserId()
        {
            // Act
            var result = _claimsService.GetUserIdFromClaim();

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(123));
        }

        [Test]
        public void GetUserIdFromClaim_WithInvalidClaim_ReturnsError()
        {
            // Arrange
            _httpContext.User = null; // Simulate no user logged in

            // Act
            var result = _claimsService.GetUserIdFromClaim();

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("No user is logged in"));
        }

        [Test]
        public void GetUserRoleIdFromClaim_WithValidClaim_ReturnsUserRoleId()
        {
            // Act
            var result = _claimsService.GetUserRoleIdFromClaim();

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(1));
        }

        [Test]
        public void GetUserRoleIdFromClaim_WithInvalidClaim_ReturnsError()
        {
            // Arrange
            _httpContext.User = null; // Simulate no user logged in

            // Act
            var result = _claimsService.GetUserRoleIdFromClaim();

            // Assert
            Assert.That(result.IsT1, Is.True);
            Assert.That(result.AsT1.Message, Is.EqualTo("No user is logged in"));
        }
    }
}

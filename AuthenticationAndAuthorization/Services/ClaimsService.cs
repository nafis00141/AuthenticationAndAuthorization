using OneOf;
using System.Security.Claims;

namespace AuthenticationAndAuthorization.Services
{
    public interface IClaimsService
    {
        ClaimsPrincipal Create(long userId, int userRoleId, string? authenticationType = "CustomCookieAuthentication");
        OneOf<long, Error> GetUserIdFromClaim();
        OneOf<long, Error> GetUserRoleIdFromClaim();
    }

    public class ClaimsService : IClaimsService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private const string UserIdClaimType = "id";
        private const string UserRoleIdClaimType = "roleId";

        public ClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Create(long userId, int userRoleId, string? authenticationType = "CustomCookieAuthentication")
        {
            var identity = new ClaimsIdentity(new[] { new Claim(UserIdClaimType, $"{userId}"), new Claim(UserRoleIdClaimType, $"{userRoleId}") }, authenticationType);
            return new ClaimsPrincipal(identity);
        }

        public OneOf<long, Error> GetUserIdFromClaim()
        {
            var userIdString = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == UserIdClaimType)?.Value;

            if (userIdString == null) return Error.Create("No user is logged in");

            return long.Parse(userIdString);
        }

        public OneOf<long, Error> GetUserRoleIdFromClaim()
        {
            var userRoleIdString = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == UserRoleIdClaimType)?.Value;

            if (userRoleIdString == null) return Error.Create("No user is logged in");

            return long.Parse(userRoleIdString);
        }
    }
}

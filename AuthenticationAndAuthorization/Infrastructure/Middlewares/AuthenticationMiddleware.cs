using AuthenticationAndAuthorization.Services;

namespace AuthenticationAndAuthorizationInfrastructure.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IAuthCookieProvider authCookieProvider, IClaimsService claimsService)
        {
            var mayBeUserId = authCookieProvider.GetUserIdFromAuthCookie();

            if (mayBeUserId.IsT0)
            {
                var userId = mayBeUserId.AsT0;
                var userRoleId = authCookieProvider.GetUserRoleFromAuthCookie().AsT0;

                httpContext.User = claimsService.Create(userId, userRoleId);
            }
        
            return _next(httpContext);
        }
    }
}

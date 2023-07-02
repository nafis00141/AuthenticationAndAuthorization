using AuthenticationAndAuthorization.Services;

namespace AuthenticationAndAuthorization.Infrastructure
{
    public static class ServiceRegistry
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}

namespace AuthenticationAndAuthorization.Infrastructure
{
    public static class AuthenticationRegistry
    {
        public static void RegisterAuthenticationServices(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddScoped<IAuthCookieProvider, AuthCookieProvider>();
        }
    }
}

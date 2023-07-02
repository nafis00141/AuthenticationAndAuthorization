using AuthenticationAndAuthorization.Services;

public static class AuthenticationEndpointsExtension
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/login", (LogInDto? logInDto, IUserService userService, IAuthCookieProvider authCookieProvider) =>
        {
            if (logInDto == null || string.IsNullOrEmpty(logInDto.UserEmail)) return Results.BadRequest("No user email");

            var user = userService.GetUserByEmail(logInDto.UserEmail);

            if (user == null) return Results.NotFound();

            authCookieProvider.AddAuthCookieFor(user);

            return Results.Ok();
        });

        app.MapGet("/logout", (IAuthCookieProvider authCookieProvider) =>
        {
            authCookieProvider.RemoveAuthCookie();
            return Results.Ok();
        });

        app.MapGet("/currentUser", (ICurrentUserService currentUserService) =>
        {
            return currentUserService.Get()
                .Match(Results.Ok, Results.BadRequest);
        });

        app.MapGet("/users", (IUserService userService) =>
        {
            return Results.Ok(userService.GetUsers());
        });
    }

    public class LogInDto
    {
        public string? UserEmail { get; set;}
    }
}

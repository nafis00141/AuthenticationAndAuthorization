using AuthenticationAndAuthorization.Services;
using Microsoft.AspNetCore.Authentication;

public static class AuthenticationEndpointsExtension
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/login", async (LogInDto? logInDto, HttpContext ctx, IUserService userService, IClaimsService claimsService) =>
        {
            if (logInDto == null || string.IsNullOrEmpty(logInDto.UserEmail)) return Results.BadRequest("No user email");

            var user = userService.GetUserByEmail(logInDto.UserEmail);

            if (user == null) return Results.NotFound();

            var claimsPrincipal = claimsService.Create(user.Id, (int)user.Role);

            await ctx.SignInAsync(AuthenticationSchemes.CookieAuthenticationScheme, claimsPrincipal);

            return Results.Ok();
        });

        app.MapGet("/logout", async (HttpContext ctx) =>
        {
            //authCookieProvider.RemoveAuthCookie();
            await ctx.SignOutAsync();
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

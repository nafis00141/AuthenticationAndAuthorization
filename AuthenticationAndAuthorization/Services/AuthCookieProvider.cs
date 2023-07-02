using AuthenticationAndAuthorization.Services;
using AuthenticationAndAuthorizationInfrastructure;
using AuthenticationAndAuthorizationInfrastructure.Result;
using Microsoft.AspNetCore.DataProtection;
using OneOf;

public interface IAuthCookieProvider
{
    void AddAuthCookieFor(User user);
    OneOf<long, Error> GetUserIdFromAuthCookie();
    OneOf<int, Error> GetUserRoleFromAuthCookie();
    void RemoveAuthCookie();
}

public class AuthCookieProvider : IAuthCookieProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private const string UserId = "id";
    private const string UserRole = "role";
    private const string DataProtectionPurpose = "auth-cookie-protection";

    public AuthCookieProvider(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public void AddAuthCookieFor(User user)
    {
        var ctx = _httpContextAccessor.HttpContext;
        var idp = _dataProtectionProvider.CreateProtector(DataProtectionPurpose);
        var cookie = AuthCookieService.CreateAuthCookie(new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>(UserId, $"{user.Id}"),
            new KeyValuePair<string, string>(UserRole, $"{(int)user.Role}")
        }, idp.Protect);

        CookieOptions options = new()
        {
            Expires = DateTime.Now.AddHours(2),
            HttpOnly = true
        };

        ctx?.Response.Cookies.Append(cookie.Key, cookie.Value, options);
    }

    public void RemoveAuthCookie()
    {
        var ctx = _httpContextAccessor.HttpContext;

        if(ctx != null) 
        {
            AuthCookieService.RemoveAuthCookie(ctx.Response.Cookies.Delete);
        }
    }

    public OneOf<long, Error> GetUserIdFromAuthCookie()
    {
        var idp = _dataProtectionProvider.CreateProtector(DataProtectionPurpose);
        var cookies = _httpContextAccessor.HttpContext?.Request.Cookies.ToList();
        return AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, UserId, idp.Unprotect)
            .MapT0(keyValue => long.Parse(keyValue.Value));
    }

    public OneOf<int, Error> GetUserRoleFromAuthCookie()
    {
        var idp = _dataProtectionProvider.CreateProtector(DataProtectionPurpose);
        var cookies = _httpContextAccessor.HttpContext?.Request.Cookies.ToList();
        return AuthCookieService.GetKeyValueFromAuthCookieByKey(cookies, UserRole, idp.Unprotect)
            .MapT0(keyValue => int.Parse(keyValue.Value));
    }
}
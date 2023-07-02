using OneOf;

namespace AuthenticationAndAuthorization.Services
{
    public interface ICurrentUserService
    {
        OneOf<User, Error> Get();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IClaimsService _claimsService;
        private readonly IUserService _userService;

        public CurrentUserService(IUserService userService, IClaimsService claimsService)
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        public OneOf<User, Error> Get()
        {
            var maybeUserId = _claimsService.GetUserIdFromClaim();

            if (maybeUserId.IsT1) return maybeUserId.AsT1;

            var user = _userService.GetUserById(maybeUserId.AsT0);

            if (user == null) return Error.Create("No user is found with id " + maybeUserId.AsT0);

            return user;
        }
    }
}

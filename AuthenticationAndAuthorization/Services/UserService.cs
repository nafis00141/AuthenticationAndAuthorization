namespace AuthenticationAndAuthorization.Services
{
    public interface IUserService
    {
        public User? GetUserById(long id);
        public User? GetUserByEmail(string email);
        public IEnumerable<User> GetUsers();

    }

    public class UserService : IUserService
    {
        private readonly IEnumerable<User> _users;

        public UserService()
        {
            _users = new List<User>()
            {
               new User(1, "Nafis", "nafis0014@gmail.com", UserRole.Admin, true),
               new User(2, "Sajid", "sajid123@gmail.com", UserRole.User, true)
            };
        }

        public User? GetUserByEmail(string email) =>
          _users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

        public User? GetUserById(long id) =>
          _users.FirstOrDefault(x => x.Id == id);

        public IEnumerable<User> GetUsers() => _users;
    }

    public record User(
      long Id,
      string Name,
      string Email,
      UserRole Role,
      bool Active
    );

    public enum UserRole
    {
        None,
        Admin,
        User
    }
}

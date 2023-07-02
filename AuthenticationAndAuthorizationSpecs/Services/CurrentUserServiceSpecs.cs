using AuthenticationAndAuthorization.Services;
using AuthenticationAndAuthorizationInfrastructure.Result;
using FakeItEasy;
using OneOf;

namespace AuthenticationAndAuthorizationSpecs.Services
{
    [TestFixture]
    public class CurrentUserServiceSpecs
    {
        private CurrentUserService _currentUserService;
        private IUserService _userService;
        private IClaimsService _claimsService;

        [SetUp]
        public void Setup()
        {
            // Create fake implementations of IUserService and IClaimsService using a mocking framework like FakeItEasy
            _userService = A.Fake<IUserService>();
            _claimsService = A.Fake<IClaimsService>();

            // Create an instance of CurrentUserService with the fake implementations
            _currentUserService = new CurrentUserService(_userService, _claimsService);
        }

        [Test]
        public void Get_WithValidUserId_ReturnsUser()
        {
            // Arrange
            long userId = 1;
            User expectedUser = new(1, "Nafis", "nafis0014@gmail.com", UserRole.User, true);

            // Mock the GetUserIdFromClaim method to return a valid user ID
            A.CallTo(() => _claimsService.GetUserIdFromClaim())
                .Returns(OneOf<long, Error>.FromT0(userId));

            // Mock the GetUserById method to return the expected user
            A.CallTo(() => _userService.GetUserById(userId))
                .Returns(expectedUser);

            // Act
            var result = _currentUserService.Get();

            // Assert
            Assert.That(result.IsT0);
            Assert.That(result.AsT0, Is.EqualTo(expectedUser));
        }

        [Test]
        public void Get_WithInvalidUserId_ReturnsError()
        {
            // Arrange
            long userId = 1;

            // Mock the GetUserIdFromClaim method to return an error
            A.CallTo(() => _claimsService.GetUserIdFromClaim())
                .Returns(OneOf<long, Error>.FromT1(Error.Create("No user is logged in")));

            // Act
            var result = _currentUserService.Get();

            // Assert
            Assert.That(result.IsT1);
            Assert.That(result.AsT1.Message, Is.EqualTo("No user is logged in"));
        }
    }
}
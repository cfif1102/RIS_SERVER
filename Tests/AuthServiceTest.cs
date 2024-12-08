using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.common;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.user;
using RIS_SERVER.src.user.dto;

namespace Tests
{
    public class AuthServiceTest
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthServiceTest()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _configuration = configuration;
            _context = CreateInMemoryDbContext();
            _tokenService = new TokenService(_configuration);
            _userService = new UserService(_context, _tokenService);
            _authService = new AuthService(_userService, _tokenService);

        }

        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void CheckNoUserSignIn()
        {

            var exception = Assert.Throws<WsException>(() =>
            {
                var user = _authService.SignIn(new CreateUserDto
                {
                    Username = "no_user@gmail.com",
                    Password = "Zxcvbnm_123"
                });
            });
        }


        [Fact]
        public void CheckUserExistSignUp()
        {
            var user = _authService.SignUp(new CreateUserDto
            {
                Username = "john",
                Password = "Zxcvbnm_123"
            });

            Assert.NotNull(user);

            Assert.Throws<WsException>(() =>
            {
                var user = _authService.SignIn(new CreateUserDto
                {
                    Username = "john",
                    Password = "Zxcvbnm_1234"
                });
            });

            Assert.Throws<WsException>(() =>
            {
                var user = _authService.SignIn(new CreateUserDto
                {
                    Username = "no_user@gmail.com",
                    Password = "Zxcvbnm_1234"
                });
            });

            _authService.SignIn(new CreateUserDto
            {
                Username = "john",
                Password = "Zxcvbnm_123"
            });
        }

        [Fact]
        public void CheckSignInToAccountThatNotExists()
        {
            Assert.Throws<WsException>(() =>
            {
                var user = _authService.SignIn(new CreateUserDto
                {
                    Username = "artem@gmail.com",
                    Password = "Zxcvbnm_1234"
                });
            });
        }

        [Fact]
        public void CheckIncorrectSignUpDto()
        {
            Assert.Throws<WsException>(() =>
            {
                DtoValidator.Validate<CreateUserDto>("{\"name\": \"test\"}");
            });
        }

        [Fact]
        public void CheckBadJwtToken()
        {
            var token = _tokenService.ValidateToken("sdgfdjflklfg");

            Assert.Null(token);
        }

        [Fact]
        public void ChecGoodJwtToken()
        {
            var user = _authService.SignUp(new CreateUserDto
            {
                Username = "cfif1112",
                Password = "Zxcvbnm_123"
            });

            Assert.NotNull(user);

            var token = _tokenService.ValidateToken(user.Token);

            Assert.NotNull(token);
        }
    }
}
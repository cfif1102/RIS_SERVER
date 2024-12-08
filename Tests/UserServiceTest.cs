using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.user;
using RIS_SERVER.src.user.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class UserServiceTest
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public UserServiceTest()
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
        public void FindNoUser()
        {
            Assert.Throws<WsException>(() =>
            {
                _userService.FindOne(999);
            });
        }

        [Fact]
        public void FindNullUserByUsername()
        {
            var user = _userService.FindByUsername("no_user");

            Assert.Null(user);
        }

        [Fact]
        public void TryExtractByIncorrectToken()
        {
            Assert.Throws<WsException>(() =>
            {
                _userService.Me("no_token");
            });
        }

        [Fact]
        public void CheckEmptyUsers()
        {
            var users = _userService.FindMany();

            Assert.Equal(0, users.Count());
        }

        [Fact]
        public void CheckUserCreated()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "check",
                Password = "test"
            });

            var found = _userService.FindByUsername("check");
               
            Assert.Equal(user.Id, found.Id);
        }
    }
}

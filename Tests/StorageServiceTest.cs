using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.common;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.storage;
using RIS_SERVER.src.storage.dto;
using RIS_SERVER.src.user;
using RIS_SERVER.src.user.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    public class StorageServiceTest
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly StorageService _storageService;

        public StorageServiceTest()
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
            _storageService = new StorageService(_userService, _context);
        }

        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void CheckStorageDto()
        {
            Assert.Throws<JsonException>(() =>
            {
                DtoValidator.Validate<CreateStorageDto>("INCORRECT DATA");
            });
        }

        [Fact]
        public void CheckCreateStorage()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var storage = _storageService.Create(new CreateStorageDto
            {
                UserId = user.Id,
                Name = "test storage"
            });

            var found = _storageService.FindById(storage.Id);

            Assert.NotNull(found);
        }

        [Fact]
        public void CheckUpdateStorage()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var storage = _storageService.Create(new CreateStorageDto
            {
                UserId = user.Id,
                Name = "test storage"
            });

            var updated = _storageService.Update(storage.Id, new UpdateStorageDto
            {
                Name = "test",
            });

            Assert.Equal(updated.Name, "test");
        }

        [Fact]
        public void CheckNotExistedStorage()
        {
            Assert.Throws<WsException>(() =>
            {
                _storageService.FindById(999);
            });
        }

        [Fact]
        public void DeleteNotExistedStorage()
        {
            Assert.Throws<WsException>(() =>
            {
                _storageService.Delete(999);
            });
        }

        [Fact]
        public void DeleteRealStorage()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var storage = _storageService.Create(new CreateStorageDto
            {
                UserId = user.Id,
                Name = "test storage"
            });
            
            _storageService.Delete(storage.Id);

            Assert.Throws<WsException>(() =>
            {
                _storageService.FindById(storage.Id);
            });
        }

        [Fact]
        public void TryAddCollaborator()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var userColl = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var storage = _storageService.Create(new CreateStorageDto
            {
                UserId = user.Id,
                Name = "test storage"
            });

            _storageService.AddCollaborator(new CollaboratorDto
            {
                UserId = userColl.Id,
                StorageId = storage.Id
            });

            var storageFound = _storageService.FindOne(storage.Id);

            Assert.True(storageFound.Collaborators.Any(user => userColl.Id == user.Id));
        }

        [Fact]
        public void TryRemoveCollaborator()
        {
            var user = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var userColl = _userService.Create(new CreateUserDto
            {
                Username = "test",
                Password = "test"
            });

            var storage = _storageService.Create(new CreateStorageDto
            {
                UserId = user.Id,
                Name = "test storage"
            });

            _storageService.AddCollaborator(new CollaboratorDto
            {
                UserId = userColl.Id,
                StorageId = storage.Id
            });

            _storageService.RemoveCollaborator(new CollaboratorDto
            {
                UserId = userColl.Id,
                StorageId = storage.Id
            });

            var storageFound = _storageService.FindOne(storage.Id);

            Assert.False(storageFound.Collaborators.Any(user => userColl.Id == user.Id));
        }
    }
}

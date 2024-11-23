using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.common;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.user.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.user
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public UserService(AppDbContext appDbContext, TokenService tokenService)
        {
            _context = appDbContext;
            _tokenService = tokenService;
        }

        public UserDto FindOne(int id)
        {
            var user = _context.Users.Where(user => user.Id == id).FirstOrDefault();

            if (user == null)
            {
                throw new WsException(404, "User not found...");
            }

            return new UserDto(user);
        }

        public User? FindByUsername(string username)
        {
            var user = _context.Users.Where(user => user.Username == username).FirstOrDefault();

            return user;
        }

        public User FindById(int id)
        {
            var user = _context.Users.Where(user => user.Id == id).FirstOrDefault();

            if (user == null)
            {
                throw new WsException(404, "User not found...");
            }

            return user;
        }

        public User Create(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password,
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public List<UserDto> FindMany()
        {
            var users = _context.Users.ToList();
            var dtos = new List<UserDto>();

            foreach (var user in users)
            {
                dtos.Add(new UserDto(user));
            }

            return dtos;
        }

        public UserDto Me(string token)
        {
            var t = _tokenService.ValidateToken(token);

            if (t == null)
            {
                throw new WsException(400, "Can't extract user...");
            }

            var user = FindByUsername(t.Identity.Name);

            if (user == null)
            {
                throw new WsException(400, "User not found...");
            }

            return new UserDto(user);
        }
    }
}

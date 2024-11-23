using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.common;
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

        public UserService(AppDbContext appDbContext)
        {
            _context = appDbContext;
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
    }
}

using RIS_SERVER.src.common;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.user.dto;
using RIS_SERVER.src.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIS_SERVER.server;

namespace RIS_SERVER.src.auth
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthService(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public UserAuthDto SignUp(CreateUserDto createUserDto)
        {
            var userExist = _userService.FindByUsername(createUserDto.Username);

            if (userExist != null)
            {
                throw new WsException(400, "Username is taken...");
            }

            var passwordHashed = PasswordHasher.HashPassword(createUserDto.Password);

            createUserDto.Password = passwordHashed;

            var user = _userService.Create(createUserDto);
            var token = _tokenService.GenerateToken(user.Username);
            var userDto = _userService.FindOne(user.Id);

            return new UserAuthDto(userDto, token);
        }

        public UserAuthDto SignIn(CreateUserDto signInDto)
        {
            var user = _userService.FindByUsername(signInDto.Username);

            if (user == null)
            {
                throw new WsException(400, "Incorrect username or password...");
            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(signInDto.Password, user.Password);

            if (!isPasswordValid)
            {
                throw new WsException(400, "Incorrect username or password...");
            }

            var token = _tokenService.GenerateToken(user.Username);
            var userDto = _userService.FindOne(user.Id);

            return new UserAuthDto(userDto, token);
        }
    }
}

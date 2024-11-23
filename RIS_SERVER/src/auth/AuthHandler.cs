using RIS_SERVER.server;
using RIS_SERVER.src.common;
using RIS_SERVER.src.user.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.auth
{
    public class AuthHandler
    {
        private readonly AuthService _authService;

        public AuthHandler(AuthService authService)
        {
            _authService = authService;
        }

        public object SignIn(ClientRequest request)
        {
            var signInDto = DtoValidator.Validate<CreateUserDto>(request.Data.ToString());
            var result = _authService.SignIn(signInDto);

            return new
            {
                Action = "auth/sign-in-response",
                result.Token,
                data = result.User
            };
        }

        public object SignUp(ClientRequest request)
        {
            var signUpDto = DtoValidator.Validate<CreateUserDto>(request.Data.ToString());
            var result = _authService.SignUp(signUpDto);

            return new
            {
                Action = "auth/sign-up-response",
                result.Token,
                Data = result.User
            };
        }
    }
}

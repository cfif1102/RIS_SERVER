using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.user
{
    public class UserHandler
    {
        private readonly UserService _userService;

        public UserHandler(UserService userService) 
        { 
            _userService = userService; 
        }

        public object Me(ClientRequest request)
        {
            var result = _userService.Me(request.Token);

            return new
            {
                Action = "user/me-response",
                Data = result
            };
        }

        public object FindMany(ClientRequest request)
        {
            var users = _userService.FindMany();

            return new
            {
                Action = "user/find-many-response",
                Data = new
                {
                    Items = users
                }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.user.dto
{
    public class UserAuthDto
    {
        public UserDto User { get; set; }

        public string Token { get; set; }

        public UserAuthDto(UserDto userDto, string token)
        {
            User = userDto;
            Token = token;
        }
    }
}

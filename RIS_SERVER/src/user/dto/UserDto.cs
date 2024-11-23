using RIS_SERVER.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.user.dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public UserDto(User user)
        {
            this.Username = user.Username;
            this.Id = user.Id;
        }
    }
}

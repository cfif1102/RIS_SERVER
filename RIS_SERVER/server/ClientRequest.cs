using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public class ClientRequest
    {
        public string Action { get; set; }
        public string? Token { get; set; } = null;

        public object? Data { get; set; } = null;
    }
}

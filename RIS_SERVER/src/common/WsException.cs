using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.common
{
    using System;

    public class WsException : Exception
    {
        public int ErrorCode { get; set; }
        public string CustomMessage { get; set; }

        public WsException(int errorCode, string message) : base(message)  
        {
            ErrorCode = errorCode;
            CustomMessage = message;
        }
    }

}

using RIS_SERVER.server;
using RIS_SERVER.src.jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.common.middlewares
{
    public class CheckUserMiddleware : Middleware
    {
        private readonly TokenService _tokenService;

        public CheckUserMiddleware(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public override bool Run(ClientRequest request)
        {
            var t = _tokenService.ValidateToken(request.Token);

            if (t == null)
            {
                throw new WsException(400, "Can't validate your token...");
            }

            return true;
        }
    }
}

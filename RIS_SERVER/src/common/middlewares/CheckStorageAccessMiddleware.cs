using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.storage;
using RIS_SERVER.src.storage.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.common.middlewares
{
    public class CheckStorageAccessMiddleware : Middleware
    {
        private readonly AuthService _authService;
        private readonly StorageService _storageService;

        public CheckStorageAccessMiddleware(AuthService authService, StorageService storageService)
        {
            _authService = authService;
            _storageService = storageService;
        }

        public override bool Run(ClientRequest request)
        {
            var data = DtoValidator.Validate<StorageParamsDto>(request.Data.ToString());
            var user = _authService.Me(request.Token);
            var storage = _storageService.FindById(data.StorageId);

            if (storage.OwnerId != user.Id && !storage.Collaborators.Any(collab => collab.Id == user.Id))
            {
                throw new WsException(403, "You can't access this storage cause you are neither owner, nor collaborator");
            }

            return true;
        }
    }
}

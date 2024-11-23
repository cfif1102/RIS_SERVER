using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.common;
using RIS_SERVER.src.storage.dto;
using RIS_SERVER.src.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.storage
{
    public class StorageHandler
    {
        private readonly StorageService _storageService;
        private readonly AuthService _authService;

        public StorageHandler(StorageService storageService, AuthService authService)
        {
            _storageService = storageService;
            _authService = authService;
        }

        public object Create(ClientRequest request)
        {
            var createStorageDto = DtoValidator.Validate<CreateStorageDto>(request.Data.ToString());
            var user = _authService.Me(request.Token);

            createStorageDto.UserId = user.Id;

            var storage = _storageService.Create(createStorageDto);

            return new
            {
                Action = "storage/create-response",
                Data = storage
            };
        }

        public object Update(ClientRequest request)
        {
            var updateStorageDto = DtoValidator.Validate<UpdateStorageDto>(request.Data.ToString());
            var storage = _storageService.Update(updateStorageDto.StorageId, updateStorageDto);

            return new
            {
                Action = "storage/update-response",
                Data = storage
            };
        }

        public object Delete(ClientRequest request)
        {
            var storageParamsDto = DtoValidator.Validate<StorageParamsDto>(request.Data.ToString());

            _storageService.Delete(storageParamsDto.StorageId);

            return new
            {
                Action = "storage/delete-response",
                Data = new 
                {
                    Message = "Storage is deleted..."
                }
            };
        }

        public object AddCollaborator(ClientRequest request)
        {
            var collaboratorDto = DtoValidator.Validate<CollaboratorDto>(request.Data.ToString());
            var storage = _storageService.AddCollaborator(collaboratorDto);

            return new
            {
                Action = "storage/add-collaborator-response",
                Data = storage
            };
        }

        public object RemoveCollaborator(ClientRequest request)
        {
            var collaboratorDto = DtoValidator.Validate<CollaboratorDto>(request.Data.ToString());
            var storage = _storageService.RemoveCollaborator(collaboratorDto);

            return new
            {
                Action = "storage/remove-collaborator-response",
                Data = storage
            };
        }
    }
}

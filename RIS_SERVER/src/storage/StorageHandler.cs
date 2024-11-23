using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.common;
using RIS_SERVER.src.storage.dto;
using RIS_SERVER.src.user;

namespace RIS_SERVER.src.storage
{
    public class StorageHandler
    {
        private readonly StorageService _storageService;
        private readonly UserService _userService;

        public StorageHandler(StorageService storageService, UserService userService)
        {
            _storageService = storageService;
            _userService = userService;
        }

        public object FindUserStorages(ClientRequest request)
        {
            var user = _userService.Me(request.Token);
            var storages = _storageService.FindUserStorages(user.Id);

            return new
            {
                Action = "storage/find-many-user-response",
                Data = new
                {
                    Items = storages
                }
            };
        }

        public object FindMany(ClientRequest request)
        {
            var storages = _storageService.FindMany();

            return new
            {
                Action = "storage/find-many-response",
                Data = new
                {
                    Items = storages
                }
            };
        }

        public object Create(ClientRequest request)
        {
            var createStorageDto = DtoValidator.Validate<CreateStorageDto>(request.Data.ToString());
            var user = _userService.Me(request.Token);

            createStorageDto.UserId = user.Id;

            var storage = _storageService.Create(createStorageDto);
            var storageDto = _storageService.FindOne(storage.Id);

            return new
            {
                Action = "storage/create-response",
                Data = storageDto
            };
        }

        public object Update(ClientRequest request)
        {
            var updateStorageDto = DtoValidator.Validate<UpdateStorageDto>(request.Data.ToString());
            var storage = _storageService.Update(updateStorageDto.StorageId, updateStorageDto);
            var storageDto = _storageService.FindOne(storage.Id);

            return new
            {
                Action = "storage/update-response",
                Data = storageDto
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
            var storageDto = _storageService.FindOne(storage.Id);

            return new
            {
                Action = "storage/add-collaborator-response",
                Data = storageDto
            };
        }

        public object RemoveCollaborator(ClientRequest request)
        {
            var collaboratorDto = DtoValidator.Validate<CollaboratorDto>(request.Data.ToString());
            var storage = _storageService.RemoveCollaborator(collaboratorDto);
            var storageDto = _storageService.FindOne(storage.Id);

            return new
            {
                Action = "storage/remove-collaborator-response",
                Data = storageDto
            };
        }
    }
}

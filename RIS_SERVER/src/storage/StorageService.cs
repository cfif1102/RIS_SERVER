using Microsoft.EntityFrameworkCore;
using RIS_SERVER.entities;
using RIS_SERVER.server;
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
    public class StorageService
    {
        private readonly UserService _userService;
        private readonly AppDbContext _context;

        public StorageService(UserService userService, AppDbContext context)
        {
            _context = context;
            _userService = userService;
        }

        public Storage AddCollaborator(CollaboratorDto collaboratorDto)
        {
            var user = _userService.FindById(collaboratorDto.UserId);
            var storage = FindById(collaboratorDto.StorageId);

            if (storage.OwnerId == user.Id)
            {
                throw new WsException(400, "You can't work with yourself as collaborators...");
            }

            if (storage.Collaborators.Any(coll => coll.Id == user.Id))
            {
                throw new WsException(400, "The user is already a collaborator, so you can't add him again...");
            }

            storage.Collaborators.Add(user);

            _context.SaveChanges();

            return storage;
        }

        public Storage RemoveCollaborator(CollaboratorDto collaboratorDto)
        {
            var user = _userService.FindById(collaboratorDto.UserId);
            var storage = FindById(collaboratorDto.StorageId);

            if (storage.OwnerId == user.Id)
            {
                throw new WsException(400, "You can't work with yourself as collaborators...");
            }

            if (storage.Collaborators.Any(coll => coll.Id == user.Id) == false)
            {
                throw new WsException(400, "The user isn't a collaborator, so you can't remove him...");
            }

            storage.Collaborators.Remove(user);

            _context.SaveChanges();

            return storage;
        }

        public Storage FindById(int id)
        {
            var storage = _context.Storages
                .Include(storage => storage.Collaborators)
                .Where(storage => storage.Id == id)
                .FirstOrDefault();

            if (storage == null)
            {
                throw new WsException(404, "Storage not found");
            }

            return storage;
        }

        public Storage Create(CreateStorageDto createStorageDto)
        {
            var user = _userService.FindById(createStorageDto.UserId);
            var storage = new Storage
            {
                Owner = user,
                Name = createStorageDto.Name,
            };

            _context.Storages.Add(storage);
            _context.SaveChanges();

            return storage;
        }

        public Storage Update(int id, UpdateStorageDto updateStorageDto)
        {
            var storage = FindById(id);

            storage.Name = updateStorageDto.Name;

            _context.SaveChanges();

            return storage;
        }

        public void Delete(int id)
        {
            var storage = FindById(id);

            _context.Storages.Remove(storage);
            _context.SaveChanges();
        }
    }
}

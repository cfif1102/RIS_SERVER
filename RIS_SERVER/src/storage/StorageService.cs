using RIS_SERVER.entities;
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

        public Storage FindById(int id)
        {
            var storage = _context.Storages.Where(storage => storage.Id == id).FirstOrDefault();

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

        public Storage Update(int id, CreateStorageDto createStorageDto)
        {
            var storage = FindById(id);

            storage.Name = createStorageDto.Name;

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

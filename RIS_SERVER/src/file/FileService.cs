using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.common;
using RIS_SERVER.src.file.dto;
using RIS_SERVER.src.storage;
using RIS_SERVER.src.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = RIS_SERVER.entities.File;

namespace RIS_SERVER.src.file
{
    public class FileService
    {
        private readonly AppDbContext _context;
        private readonly StorageService _storageService;

        public FileService(AppDbContext context, StorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public File FindById(int id)
        {
            var file = _context.Files.Where(file =>  file.Id == id).FirstOrDefault();

            if (file == null)
            {
                throw new WsException(404, "File not found...");
            }

            return file;
        }

        public File Create(CreateFileDto createFileDto)
        {
            var storage = _storageService.FindById(createFileDto.StorageId);
            var file = new File
            {
                Name = createFileDto.Name,
                Path = createFileDto.Path,
                Size = createFileDto.Size,
                Storage = storage
            };

            _context.Files.Add(file);
            _context.SaveChanges();

            return file;
        }

        public File Update(int id, UpdateFileDto updateFileDto)
        {
            var file = FindById(id);

            file.Name = updateFileDto.Name;

            _context.SaveChanges();

            return file;
        }

        public void Delete(int id)
        {
            var file = FindById(id);

            _context.Files.Remove(file);
            _context.SaveChanges();
        }
    }
}

using RIS_SERVER.entities;
using RIS_SERVER.src.file.dto;
using RIS_SERVER.src.user.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.storage.dto
{
    public class StorageDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public UserDto Owner { get; set; }

        public List<UserDto> Collaborators { get; set; } = [];

        public List<FileDto> Files { get; set; } = [];

        public StorageDto(Storage storage) 
        {
            Id = storage.Id;
            Name = storage.Name;
            Owner = new UserDto(storage.Owner);

            foreach (var user in storage.Collaborators)
            {
                Collaborators.Add(new UserDto(user));
            }

            foreach (var file in storage.Files)
            {
                Files.Add(new FileDto(file));
            }
        }
    }
}

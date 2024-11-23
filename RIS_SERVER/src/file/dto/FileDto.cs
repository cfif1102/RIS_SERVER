using RIS_SERVER.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = RIS_SERVER.entities.File;

namespace RIS_SERVER.src.file.dto
{
    public class FileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public FileDto(File file)
        {
            Id = file.Id;
            Name = file.Name;
            Size = file.Size;
        }
    }
}

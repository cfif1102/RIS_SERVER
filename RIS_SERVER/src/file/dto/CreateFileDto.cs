using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file.dto
{
    public class CreateFileDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Size { get; set; }

        public string Path { get; set; }

        [Required]
        public string File { get; set; }

        [Required]
        public int StorageId { get; set; }
    }
}

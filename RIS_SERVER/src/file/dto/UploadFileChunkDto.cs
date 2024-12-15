using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file.dto
{
    public class UploadFileChunkDto
    {
        [Required]
        public string Chunk { get; set; }

        [Required]
        public int StorageId { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}

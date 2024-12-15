using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file.dto
{
    public class DownloadFileChunkDto
    {
        [Required]
        public int Offset { get; set; }

        [Required]
        public int FileId { get; set; }
    }
}

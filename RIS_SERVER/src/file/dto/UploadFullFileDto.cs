using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file.dto
{
    public class UploadFullFileDto
    {
        [Required]
        public int StorageId { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public string RealName { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}

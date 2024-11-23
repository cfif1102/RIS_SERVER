using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file.dto
{
    public class FileParamsDto
    {
        [Required]
        public int FileId { get; set; }
    }
}

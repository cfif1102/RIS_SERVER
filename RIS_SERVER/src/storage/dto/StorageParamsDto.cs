using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.storage.dto
{
    public class StorageParamsDto
    {

        [Required]
        public int StorageId { get; set; }
    }
}

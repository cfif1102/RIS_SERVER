using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.entities
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public string Path { get; set; }
        public Storage Storage { get; set; }
        public int StorageId { get; set; }
    }
}

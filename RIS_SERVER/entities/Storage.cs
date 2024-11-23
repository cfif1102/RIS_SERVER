using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.entities
{
    public class Storage
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<File> Files { get; set; }

        public User Owner { get; set; }

        public int OwnerId { get; set; }
        public List<User> Collaborators { get; set; }

    }
}

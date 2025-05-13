using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public class Image
    {
        public int Id { get; set; }

        public required string Path { get; set; }

        public required string Name { get; set; }    

        public List<Event> Events { get; set; } = new ();
    }
}

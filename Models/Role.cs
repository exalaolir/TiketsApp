using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Surname { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public DateTime DateOfRegistration { get; set; }

        public bool? BannedByAdmin { get; set; } = false;
    }
}

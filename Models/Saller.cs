using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public sealed class Saller : Role
    {
        public string? Number { get; set; }

        public bool? IsNowRegister { get; set; } = false;

        public List<Event> Events { get; set; } = new List<Event>();

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

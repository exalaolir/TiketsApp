using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models.DTO
{
    class PriviewEventDto(User user, Event newEvent)
    {
        public User User => user;
        public Event Event => newEvent; 
    }
}

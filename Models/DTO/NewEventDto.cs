using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models.DTO
{
    class NewEventDto(Models.Saller saller, Models.Event? newEvent)
    {
        public Saller Saller => saller;
        public Models.Event? Event => newEvent;
    }
}

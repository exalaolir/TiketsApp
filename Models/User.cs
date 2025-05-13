using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public sealed class User : Role
    {
        public DateTime Birthday { get; set; }
    }
}

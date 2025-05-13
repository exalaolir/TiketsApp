using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public class Admin : Role
    {
        public bool? HasAllPermissions { get; set; } = true;
    }
}

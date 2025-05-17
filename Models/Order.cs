using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public enum Status
    {
        Whait,
        Changed,
        Banned,
        Succes,
        RejectByUser
    }

    public class Order
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Event? Event { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int SallerId { get; set; }

        public Saller? Saller { get; set; }

        public int? Row { get; set; }
        public int? Seat { get; set; }

        public decimal Cost { get; set; }

        public Status Status { get; set; }
    }
}

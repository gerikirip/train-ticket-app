using System;
using System.Collections.Generic;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public int? Wagon { get; set; }
        public int? SeatNumber { get; set; }
        public string TicketId { get; set; }
        public int TimeId { get; set; }
        public int OwnerId { get; set; }
        public int Price { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual TrainTime Time { get; set; }
    }
}

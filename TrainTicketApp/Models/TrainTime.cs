using System;
using System.Collections.Generic;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class TrainTime
    {
        public TrainTime()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public int TrainId { get; set; }
        public int DayId { get; set; }
        public TimeSpan Time { get; set; }
        public int? TicketTypeId { get; set; }
        public int? Wagon { get; set; }
        public int? SeatCount { get; set; }
        public string TakenSeats { get; set; }

        public virtual Day Day { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Train Train { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}

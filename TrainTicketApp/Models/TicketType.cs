using System;
using System.Collections.Generic;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class TicketType
    {
        public TicketType()
        {
            TrainTimes = new HashSet<TrainTime>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ExtraFee { get; set; }

        public virtual ICollection<TrainTime> TrainTimes { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class Day
    {
        public Day()
        {
            TrainTimes = new HashSet<TrainTime>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<TrainTime> TrainTimes { get; set; }
    }
}

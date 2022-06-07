using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainTicketApp.Models;

namespace TrainTicketApp.Service
{
    public class ChartService
    {
        public Dictionary<string, int> CreatePieChartData(IQueryable<Ticket> tickets)
        {
            Dictionary<string, int> ticketStats = new Dictionary<string, int>();
            foreach (var ticket in tickets)
            {
                string type = ticket.Time.TicketType.Name;
                if (!ticketStats.ContainsKey(type))
                {
                    ticketStats.Add(type, 1);
                }
                else
                {
                    ticketStats[type]++;
                }
            }
            return ticketStats;
        }


        public SortedDictionary<string, int> CreateLineChartData(IQueryable<Ticket> tickets)
        {
            SortedDictionary<string, int> ticketStats = new SortedDictionary<string, int>();
            foreach (var ticket in tickets)
            {
                string date = ticket.Time.Day.Date.ToShortDateString();
                if (!ticketStats.ContainsKey(date))
                {
                    ticketStats.Add(date, 1);
                }
                else
                {
                    ticketStats[date]++;
                }
            }
            return ticketStats;
        }


        public IQueryable<Ticket> FilterTickets(DateTime dateBegin, DateTime dateEnd, string origStation, string destStation)
        {
            TrainData database = new TrainData();
            IQueryable<Ticket> tickets = database.Tickets.Include(t => t.Time.TicketType).Include(t => t.Time.Day).Include(t => t.Time.Train);
            return tickets.Where(t =>
                                (t.Time.Day.Date > dateBegin.Date || (t.Time.Day.Date == dateBegin.Date && t.Time.Time >= dateBegin.TimeOfDay)) &&
                                (t.Time.Day.Date < dateEnd.Date || (t.Time.Day.Date == dateEnd.Date && t.Time.Time <= dateEnd.TimeOfDay)) &&
                                (origStation == null || t.Time.Train.OriginStation.Contains(origStation)) &&
                                (destStation == null || t.Time.Train.DestStation.Contains(destStation)));
        }
    }
}

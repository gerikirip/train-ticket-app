using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TrainTicketApp.Models;

namespace TrainTicketApp.Service
{
    public class TicketService
    {
        private TrainData database = new TrainData();
        private AppService appService = new AppService();
        public Ticket NewTicket(int ownerId, int trainTimeId, int? wagon, int? seat)
        {
            TrainTime trainTime = database.TrainTimes.Include(t => t.Train).Where(t => t.Id == trainTimeId).First();
            TicketType ticketType = database.TicketTypes.Where(t => t.Id == trainTime.TicketTypeId).First();
            Guid ticketId = Guid.NewGuid();
            Ticket ticket = new Ticket();
            ticket.OwnerId = ownerId;
            ticket.TimeId = trainTimeId;
            ticket.TicketId = ticketId.ToString();
            ticket.Price = GetTicketPrice(trainTimeId);

            if (wagon != null && seat != null)
            {
                ticket.Wagon = wagon;
                ticket.SeatNumber = seat;
            }

            database.Add(ticket);
            database.SaveChanges();
            return ticket;
        }

        public int GetTicketPrice(int trainTimeId)
        {
            TrainTime trainTime = appService.GetTrainTimeById(trainTimeId);
            int price = 0;

            if (trainTime.TicketType.ExtraFee != null)
            {
                price = trainTime.TicketType.ExtraFee.Value;
            }
            price += trainTime.Train.Distance * int.Parse(ConfigurationManager.AppSettings["PriceKM"]);

            return price;
        }

        public Ticket GetTicketByTicketId(String ticketId)
        {
            return database.Tickets.Include(ow => ow.Owner)
                .Include(t => t.Time)
                .Include(t => t.Time.Train)
                .Include(t => t.Time.Day)
                .Include(t => t.Time.TicketType)
                .Where(t => t.TicketId == ticketId).FirstOrDefault();
        }

        public string TicketTypeIdToString(int id)
        {
            return database.TicketTypes.Where(t => t.Id == id).Select(t => t.Name).First();
        }

        public void RemoveTicket(String ticketId)
        {
            Ticket ticket = GetTicketByTicketId(ticketId);

            if (ticket.Time.TicketType.Name == "Hely")
            {
                Dictionary<int, List<int>> takenSeats = appService.GetTakenSeats(ticket.Time);
                bool result = appService.DeleteSeat(ref takenSeats, ticket.Wagon.GetValueOrDefault(), ticket.SeatNumber.GetValueOrDefault());
                ticket.Time.TakenSeats = JsonConvert.SerializeObject(takenSeats);
            }
            database.Tickets.Remove(ticket);
            database.SaveChanges();
        }

        public void RemoveTicketsWithTrainTime(int trainTimeId)
        {
            List<Ticket> tickets = database.Tickets.Include(t => t.Time).Include(t => t.Time.TicketType).ToList();
            foreach (var ticket in tickets)
            {
                if (ticket.TimeId == trainTimeId)
                {
                    if (ticket.Time.TicketType.Name == "Hely")
                    {
                        Dictionary<int, List<int>> takenSeats = appService.GetTakenSeats(ticket.Time);
                        bool result = appService.DeleteSeat(ref takenSeats, ticket.Wagon.GetValueOrDefault(), ticket.SeatNumber.GetValueOrDefault());
                        ticket.Time.TakenSeats = JsonConvert.SerializeObject(takenSeats);
                    }
                    database.Tickets.Remove(ticket);
                }
            }
            database.SaveChanges();
        }


        public List<TicketType> GetTicketTypes()
        {
            return database.TicketTypes.ToList();
        }
        
    }
}

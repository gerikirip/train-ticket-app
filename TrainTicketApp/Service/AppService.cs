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
    public class AppService
    {
        private TrainData database = new TrainData();
        public int NewOwner(Owner owner)
        {
            Owner foundOwner = database.Owners.FirstOrDefault(ow => ow.FirstName == owner.FirstName && ow.LastName == owner.LastName && ow.Email == owner.Email && ow.Address == owner.Address && ow.PhoneNumber == owner.PhoneNumber);
            if (foundOwner != null)
            {
                return foundOwner.Id;
            }
            else
            {
                Owner saveOwner = new Owner();
                saveOwner.FirstName = owner.FirstName;
                saveOwner.LastName = owner.LastName;
                saveOwner.PhoneNumber = owner.PhoneNumber;
                saveOwner.Address = owner.Address;
                saveOwner.Email = owner.Email;

                database.Add(saveOwner);
                database.SaveChanges();
                return database.Owners.Where(ow => ow == saveOwner).Select(ow => ow.Id).First();
            }
        }

        public int NewDay(DateTime date)
        {
            Day foundDay = database.Days.FirstOrDefault(d => d.Date == date.Date);
            if (foundDay != null)
            {
                return foundDay.Id;
            }
            else
            {
                Day saveDay = new Day();
                saveDay.Date = date.Date;

                database.Add(saveDay);
                database.SaveChanges();
                return database.Days.Where(d => d == saveDay).Select(d => d.Id).First();
            }
        }

        public int NewTrain(Train train)
        {
            Train foundTrain = database.Trains.FirstOrDefault(tr => tr.OriginStation == train.OriginStation && tr.DestStation == train.DestStation && tr.Distance == train.Distance && tr.TravelTime == train.TravelTime);
            if (foundTrain != null)
            {
                return foundTrain.Id;
            }
            else
            {
                database.Add(train);
                database.SaveChanges();
                return database.Trains.Where(tr => tr == train).Select(tr => tr.Id).First();
            }
        }

        public bool IsNoModification(TrainTime trainTime)
        {
            TrainTime databaseTrainTime = GetTrainTimeById(trainTime.Id);
            if (databaseTrainTime.Train.OriginStation == trainTime.Train.OriginStation && databaseTrainTime.Train.DestStation == trainTime.Train.DestStation &&
                databaseTrainTime.Train.Distance == trainTime.Train.Distance && databaseTrainTime.Train.TravelTime == trainTime.Train.TravelTime &&
                databaseTrainTime.Time == trainTime.Time && databaseTrainTime.Day.Date == trainTime.Day.Date && databaseTrainTime.TicketTypeId == trainTime.TicketTypeId &&
                databaseTrainTime.Wagon == trainTime.Wagon && databaseTrainTime.SeatCount == trainTime.SeatCount)
            {
                return true;
            }
            return false;
        }

        public void NewTrainTime(int trainId, int? wagon, int? seatCount, DateTime date, TimeSpan time, int ticketTypeId)
        {
            
            TrainTime trainTime = new TrainTime();
            trainTime.DayId = NewDay(date);
            trainTime.Wagon = wagon;
            trainTime.SeatCount = seatCount;
            trainTime.TrainId = trainId;
            trainTime.Time = time;
            trainTime.TicketTypeId = ticketTypeId;
            database.Add(trainTime);
            database.SaveChanges();
        }

        public bool IsValidDate(DateTime date, TimeSpan? time)
        {
            DateTime dateNow = DateTime.Now;
            if(date.Date > dateNow.Date || (date.Date == dateNow.Date && (time == null  || time >= dateNow.TimeOfDay)))
            {
                return true;
            }
            return false;
        }

        public TrainTime GetTrainTimeById(int id)
        {
            return database.TrainTimes.Include(t => t.Train).Include(t => t.Day).Include(t => t.TicketType).Where(t => t.Id == id).FirstOrDefault();
        }

        public void ChangeTrainTime(TrainTime trainTimeInput)
        {
            TrainTime trainTime = GetTrainTimeById(trainTimeInput.Id);
            int trainId = NewTrain(trainTimeInput.Train);
            trainTime.TrainId = trainId;
            trainTime.Day.Date = trainTimeInput.Day.Date;
            trainTime.Time = trainTimeInput.Time;
            trainTime.TicketTypeId = trainTimeInput.TicketTypeId;
            trainTime.Wagon = trainTimeInput.Wagon;
            trainTime.SeatCount = trainTimeInput.SeatCount;
            database.SaveChanges();
        }

        public IQueryable<TrainTime> FilterTrainTimes(string origStation, string destStation, DateTime date, TimeSpan time)
        {
            DateTime dateNow = DateTime.Now;
            TimeSpan timeOfDay = DateTime.Now.TimeOfDay;

            if (date != new DateTime(1, 1, 1, 0, 0, 0))
            {
                return database.TrainTimes.Include(t => t.Train).Include(t => t.Day).Include(t => t.TicketType)
                       .Where(e => (origStation == null || e.Train.OriginStation.Contains(origStation)) &&
                       (destStation == null || e.Train.DestStation.Contains(destStation)) &&
                       e.Day.Date == date.Date &&
                       e.Time >= time).OrderBy(t => t.Day.Date).ThenBy(t => t.Time);
            }
            else if (date == new DateTime(1, 1, 1, 0, 0, 0) && time != new TimeSpan(0, 0, 0))
            {
                return database.TrainTimes.Include(t => t.Train).Include(t => t.Day).Include(t => t.TicketType)
                       .Where(e => (origStation == null || e.Train.OriginStation.Contains(origStation)) &&
                       (destStation == null || e.Train.DestStation.Contains(destStation)) &&
                       e.Day.Date > dateNow &&
                       e.Time >= time).OrderBy(t => t.Day.Date).ThenBy(t => t.Time);
            }
            else
            {
                return database.TrainTimes.Include(t => t.Train).Include(t => t.Day).Include(t => t.TicketType)
                       .Where(e => (origStation == null || e.Train.OriginStation.Contains(origStation)) &&
                       (destStation == null || e.Train.DestStation.Contains(destStation)) &&
                       ((e.Day.Date > dateNow) || (e.Day.Date == dateNow && e.Time >= timeOfDay)))
                       .OrderBy(t => t.Day.Date).ThenBy(t => t.Time);
            }
        }

        public Dictionary<int, List<int>> GetTakenSeats(TrainTime trainTime)
        {
            int? wagon = database.TrainTimes.Where(t => t == trainTime).Select(t => t.Wagon).FirstOrDefault();
            if (wagon == null)
            {
                return null; 
            }
            Dictionary<int, List<int>> takenSeats = new Dictionary<int, List<int>>();
            if (trainTime.TakenSeats != null)
            {
                takenSeats = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(trainTime.TakenSeats);
            }
            else
            {
                for (int i = 1; i <= wagon; i++)
                {
                    List<int> seats = new List<int>();
                    takenSeats.Add(i, seats);
                }
            }
            return takenSeats;
        }

        public bool AddToTakenSeats(ref Dictionary<int, List<int>> takenSeats, int wagon, int seat)
        {
            List<int> takenSeatList = takenSeats[wagon];
            if (!takenSeatList.Contains(seat))
            {
                takenSeatList.Add(seat);
                takenSeats[wagon] = takenSeatList;
                return true;
            }
            return false;
        }

        public bool DeleteSeat(ref Dictionary<int, List<int>> takenSeats, int wagon, int seat)
        {
            List<int> takenSeatList = takenSeats[wagon];
            if (takenSeatList.Contains(seat))
            {
                takenSeatList.Remove(seat);
                takenSeats[wagon] = takenSeatList;
                return true;
            }
            return false;
        }    
    }
}

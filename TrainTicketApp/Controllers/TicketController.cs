using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using TrainTicketApp.Models;
using System.Web;
using TrainTicketApp.Service;
using Newtonsoft.Json;

namespace TrainTicketApp.Controllers
{
    public class TicketController : Controller
    {
        private AppService appService = new AppService();
        private TicketService ticketService = new TicketService();

        public IActionResult BuyTicket(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            TrainTime trainTime = appService.GetTrainTimeById(id);


            if (trainTime == null )
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.TrainTime = trainTime;
            ViewBag.Price = ticketService.GetTicketPrice(id);

            return View();
        }

        [HttpPost]
        public IActionResult BuyTicket(Owner owner, int trainTimeId)
        {
            TrainTime trainTime = appService.GetTrainTimeById(trainTimeId);

            if (!appService.IsValidDate(trainTime.Day.Date, trainTime.Time))
            {
                return RedirectToAction("Error", "Home");
            }

            if(ModelState.IsValid)
            {
                int ownerId = appService.NewOwner(owner);
                Ticket ticket = ticketService.NewTicket(ownerId, trainTimeId, null, null);

                return RedirectToAction("BuySuccess", "Ticket", new { ticketId = ticket.TicketId });
            }

            ViewBag.TrainTime = appService.GetTrainTimeById(trainTimeId);

            return View(owner);

        }

        public IActionResult BuyTicketWithSeat(int id, string error)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            TrainTime trainTime = appService.GetTrainTimeById(id);

            if (trainTime == null)
            {
                return RedirectToAction("Error", "Home");
            }

            Dictionary<int, List<int>> takenSeats = appService.GetTakenSeats(trainTime);

            if(takenSeats == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.TakenSeats = appService.GetTakenSeats(trainTime);
            ViewBag.Price = ticketService.GetTicketPrice(id);

            ViewBag.TrainTime = trainTime;
            ViewBag.Error = error;
            return View();
        }

        [HttpPost]
        public IActionResult BuyTicketWithSeat(Owner owner, int trainTimeId, int wagon, int seat)
        {
            TrainTime trainTime = appService.GetTrainTimeById(trainTimeId);

            if (ModelState.IsValid)
            {
                int ownerId = appService.NewOwner(owner);

                Dictionary<int, List<int>> takenSeats = appService.GetTakenSeats(trainTime);
                bool result = appService.AddToTakenSeats(ref takenSeats, wagon, seat);
                if (result)
                {
                    trainTime.TakenSeats = JsonConvert.SerializeObject(takenSeats);
                    Ticket ticket = ticketService.NewTicket(ownerId, trainTimeId, wagon, seat);
                    return RedirectToAction("BuySuccess", "Ticket", new { ticketId = ticket.TicketId });
                }
                else
                {
                    return RedirectToAction("BuyTicketWithSeat", "Ticket", new { error = "Foglalt a hely!" });
                }
            }

            ViewBag.TakenSeats = appService.GetTakenSeats(trainTime);
            ViewBag.TrainTime = trainTime;
            return View(owner);
        }

        public IActionResult BuySuccess(String ticketId)
        {
            if(ticketId == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.TicketId = ticketId;
            return View();
        }

        public IActionResult GetTicket(String message)
        {
            ViewBag.Message = message;
            return View();
        }

        [ActionName("GetTicket"), HttpPost]
        public IActionResult GetTicketPost(String selectedTicketId)
        {
            Ticket ticket = ticketService.GetTicketByTicketId(selectedTicketId);
            if(ticket != null)
            {
                ViewBag.Ticket = ticket;
            }
            else
            {
                ViewBag.Error = "Nincs ilyen azonosítóval rendelés!";
            }                
            return View();
        }

        public IActionResult RemoveTicket(String ticketId)
        {
            ticketService.RemoveTicket(ticketId);
            return RedirectToAction("GetTicket", "Ticket", new { message = "Sikeres törlés" });
        }
    }
}

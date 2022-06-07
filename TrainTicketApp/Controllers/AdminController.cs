using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TrainTicketApp.Models;
using TrainTicketApp.Service;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TrainTicketApp.Controllers
{
    public class AdminController : Controller
    {
        private ChartService chartService = new ChartService();
        private TicketService ticketService = new TicketService();
        private AppService appService = new AppService();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string origStation, string destStation, DateTime date, TimeSpan time)
        {
            ViewBag.TrainTimes = appService.FilterTrainTimes(origStation, destStation, date, time);
            return View();
        }

        public IActionResult ChangeTrainTime(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            this.HttpContext.Session.SetString("TicketTypes", JsonSerializer.Serialize(ticketService.GetTicketTypes()));
            TrainTime trainTime = appService.GetTrainTimeById(id);

            if(trainTime == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(trainTime);
        }
        [HttpPost]
        public IActionResult ChangeTrainTime(TrainTime trainTime)
        {
            if (!appService.IsValidDate(trainTime.Day.Date, trainTime.Time))
            {
                ViewBag.Error = "Nem adhatsz meg a mostani időnél előbbi időpontot!";
            }
            else
            {
                if(ModelState.IsValid)
                {
                    if(!appService.IsNoModification(trainTime))
                    {
                        ticketService.RemoveTicketsWithTrainTime(trainTime.Id);
                        appService.ChangeTrainTime(trainTime);
                        ViewBag.Message = "Sikeresen módosította a járatot!";
                    }
                    else
                    {
                        ViewBag.Error = "Nem módosított semmilyen adatot!";
                    }

                }
            }
            return View(trainTime);
        }

        public IActionResult NewTrain()
        {
            this.HttpContext.Session.SetString("TicketTypes", JsonSerializer.Serialize(ticketService.GetTicketTypes()));
            return View();
        }

        [HttpPost]
        public IActionResult NewTrain(Train train, int wagon, int seatCount, DateTime date)
        {
            if (!appService.IsValidDate(date, null))
            {
                ViewBag.Error = "Nem adhatsz meg a mai napnál előbbi dátumot!";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    int trainId = appService.NewTrain(train);
                    TimeSpan time = new TimeSpan();

                    bool timeInvalid = false;
                    int i = 0;
                    foreach (var key in Request.Form.Keys)
                    {
                        string keyString = key.ToString();
                        if (keyString.StartsWith("trainTime"))
                        {
                            if(Request.Form["trainTime" + i] != "")
                            {
                                time = TimeSpan.Parse(Request.Form["trainTime" + i]);
                            }
                            else
                            {
                                timeInvalid = true;
                            }
                        }
                        else if (keyString.StartsWith("ticketType"))
                        {
                            int ticketTypeId = int.Parse(Request.Form["ticketType" + i]);
                            if (appService.IsValidDate(date, time) && !timeInvalid)
                            {
                                appService.NewTrainTime(trainId, wagon, seatCount, date, time, ticketTypeId);
                            }
                            else
                            {
                                timeInvalid = false;
                                ViewBag.Error = "Egy vagy több időpont nem került mentésre!";
                            }

                            i++;
                        }
                    }
                    ViewBag.Message = "Sikeres rögzítetés!";
                }
            }
            return View(train);
        }

        public IActionResult Chart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Chart(DateTime dateBegin, DateTime dateEnd, string origStation, string destStation)
        {
            IQueryable<Ticket> tickets = chartService.FilterTickets(dateBegin, dateEnd, origStation, destStation);
            ViewBag.TicketStats = JsonSerializer.Serialize(chartService.CreatePieChartData(tickets));

            return View();
        }

        public IActionResult LineChart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LineChart(DateTime dateBegin, DateTime dateEnd, string origStation, string destStation)
        {
            IQueryable<Ticket> tickets = chartService.FilterTickets(dateBegin, dateEnd, origStation, destStation);
            ViewBag.DayStats = JsonSerializer.Serialize(chartService.CreateLineChartData(tickets));

            return View();
        }

    }
}

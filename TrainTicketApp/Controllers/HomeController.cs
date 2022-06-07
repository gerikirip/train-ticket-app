using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrainTicketApp.Models;
using TrainTicketApp.Service;

namespace TrainTicketApp.Controllers
{
    public class HomeController : Controller
    {
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

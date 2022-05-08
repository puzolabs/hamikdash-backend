using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuzoLabs.Hamikdash.Reservations.Services;
using PuzoLabs.Hamikdash.Reservations.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IReservationService _reservationService;

        public ReservationController(ILogger<ReservationController> logger, IConfiguration configuration, IReservationService reservationService)
        {
            _logger = logger;
            _configuration = configuration;
            _reservationService = reservationService;
        }

        [HttpGet("FindAvailableTime")]
        //http://localhost:7005/api/Reservation/FindAvailableTime?type=nazir&from=4%2F20%2F2022%204%3A50%3A00%20PM&to=4%2F20%2F2022%206%3A50%3A00%20PM
        public async Task<IEnumerable<AvailableTimeDto>> FindAvailableTime(string type, DateTime from, DateTime to)
        {
            //todo: validate 'type'
            //validate from is at least now this minute and not in the past

            //get from the db\settings the working time needed for this korban type
            int workDuration = Convert.ToInt32(_configuration[$"{type}:workDuration"]);

            //get from the db\settings the cohen rest time between korbanot
            int restTime = Convert.ToInt32(_configuration["restTime"]);

            return await _reservationService.FindAvailableTime(type, from, to, workDuration, restTime);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class ReservationRepositoryTests
    {
        IReservationRepository _reservationRepository;

        [TestInitialize]
        public async Task InitConfiguration()
        {
            InitManager initManager = new InitManager();
            await initManager.Init();
            _reservationRepository = initManager.ReservationRepository;
        }

        [TestMethod]
        public async Task TestGetReservationsForAltarInTimeRangeOrderedAscending()
        {
            //IDatabase database = new Database(_options);

            DateTime from = DateTime.Now.AddDays(1);
            DateTime to = DateTime.Now.AddDays(5);
            IEnumerable<Reservation> reservations = await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(1, from, to);

        }
    }
}

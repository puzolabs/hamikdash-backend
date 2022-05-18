using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Services;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class ReservationServiceTests
    {
        [TestMethod]
        public void TestReserve()
        {
            IDatabase database = null;
            IReservationService reservationService = new ReservationService(database);
        }
    }
}

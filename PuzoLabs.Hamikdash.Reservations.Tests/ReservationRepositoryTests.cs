using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await _reservationRepository.DeleteAll();
        }

        [TestMethod]
        public async Task AddReservation_Success()
        {
            var reservation = new Reservation()
            {
                Type = KorbanTypes.Ola,
                AltarId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.Pending
            };

            Guid id = await _reservationRepository.Add(reservation);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public async Task ReservationExists_GetReservation_Success()
        {
            var reservation = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = new DateTime(2022, 8, 6, 22, 53, 13),
                EndDate = new DateTime(2022, 8, 6, 23, 13, 13),
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            Guid id = await _reservationRepository.Add(reservation);

            //act
            Reservation result = await _reservationRepository.Get(id);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reservation, result);
        }

        [TestMethod]
        public async Task OneReservationExists_TestGetReservationsForAltarInTimeRangeOrderedAscending_Success()
        {
            DateTime from = new DateTime(2022, 8, 6, 22, 53, 13);
            DateTime to = new DateTime(2022, 8, 6, 23, 13, 13);

            var reservation = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from,
                EndDate = to,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation);

            Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(2, from, to)).ToArray();

            Assert.AreEqual(1, reservations.Length);
            Assert.AreEqual(reservation, reservations[0]);
        }

        [TestMethod]
        public async Task TwoReservationsExists_TestGetReservationsForAltarInTimeRangeOrderedAscending_ReturnsTwo()
        {
            DateTime from1 = new DateTime(2022, 8, 6, 22, 53, 13);
            DateTime to1 = new DateTime(2022, 8, 6, 23, 13, 13);

            var reservation1 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from1,
                EndDate = to1,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation1);

            DateTime from2 = new DateTime(2022, 8, 7, 22, 53, 13);
            DateTime to2 = new DateTime(2022, 8, 7, 23, 13, 13);

            var reservation2 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from2,
                EndDate = to2,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation2);

            Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(2, from1, to2)).ToArray();

            Assert.AreEqual(2, reservations.Length);
            Assert.AreEqual(reservation1, reservations[0]);
            Assert.AreEqual(reservation2, reservations[1]);
        }

        [TestMethod]
        public async Task TwoAndAHalfReservationsExists_TestGetReservationsForAltarInTimeRangeOrderedAscending_ReturnsTwo()
        {
            DateTime from1 = new DateTime(2022, 8, 6, 22, 53, 13);
            DateTime to1 = new DateTime(2022, 8, 6, 23, 13, 13);

            var reservation1 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from1,
                EndDate = to1,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation1);

            DateTime from2 = new DateTime(2022, 8, 7, 22, 53, 13);
            DateTime to2 = new DateTime(2022, 8, 7, 23, 13, 13);

            var reservation2 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from2,
                EndDate = to2,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation2);

            DateTime from3 = new DateTime(2022, 8, 8, 22, 53, 13);
            DateTime to3 = new DateTime(2022, 8, 8, 23, 13, 13);

            var reservation3 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from3,
                EndDate = to3,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation3);

            Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(2, from1, from3.AddMinutes(5))).ToArray();

            Assert.AreEqual(2, reservations.Length);
            Assert.AreEqual(reservation1, reservations[0]);
            Assert.AreEqual(reservation2, reservations[1]);
        }

        [TestMethod]
        public async Task TwoAndAHalfReservationsExistsWhichTheFirstOneOfOtherAltar_TestGetReservationsForAltarInTimeRangeOrderedAscending_ReturnsTheSecondOne()
        {
            DateTime from1 = new DateTime(2022, 8, 6, 22, 53, 13);
            DateTime to1 = new DateTime(2022, 8, 6, 23, 13, 13);

            var reservation1 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 3,
                StartDate = from1,
                EndDate = to1,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation1);

            DateTime from2 = new DateTime(2022, 8, 7, 22, 53, 13);
            DateTime to2 = new DateTime(2022, 8, 7, 23, 13, 13);

            var reservation2 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from2,
                EndDate = to2,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation2);

            DateTime from3 = new DateTime(2022, 8, 8, 22, 53, 13);
            DateTime to3 = new DateTime(2022, 8, 8, 23, 13, 13);

            var reservation3 = new Reservation()
            {
                Type = KorbanTypes.Minha,
                AltarId = 2,
                StartDate = from3,
                EndDate = to3,
                UserId = Guid.NewGuid(),
                Status = ReservationStatus.InWork
            };

            await _reservationRepository.Add(reservation3);

            Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(2, from1, from3.AddMinutes(5))).ToArray();

            Assert.AreEqual(1, reservations.Length);
            Assert.AreEqual(reservation2, reservations[0]);
        }
    }
}

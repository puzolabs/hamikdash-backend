using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using PuzoLabs.Hamikdash.Reservations.Services;
using PuzoLabs.Hamikdash.Reservations.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class ReservationServiceTests
    {
        IAltarRepository _altarRepository;
        IReservationRepository _reservationRepository;
        IReservationService _reservationService;

        [TestInitialize]
        public async Task InitConfiguration()
        {
            InitManager initManager = new InitManager();
            await initManager.Init();
            
            _reservationService = initManager.ReservationService;
            _altarRepository = initManager.AltarRepository;
            _reservationRepository = initManager.ReservationRepository;
            
            await _reservationRepository.DeleteAll();
            await _altarRepository.RemoveAllAltars();
        }

        [TestMethod]
        public async Task ReservationsExistsForASpecificAltar_FindAvailableTimeForThatAltar_Success()
        {
            int firstAltarId = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });

            Guid userId = Guid.NewGuid();

            DateTime from1 = new DateTime(2022, 8, 7, 8, 0, 0);
            DateTime to1 = new DateTime(2022, 8, 7, 8, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from1, To = to1 }, userId);

            DateTime from2 = new DateTime(2022, 8, 7, 13, 0, 0);
            DateTime to2 = new DateTime(2022, 8, 7, 13, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from2, To = to2 }, userId);

            DateTime from3 = new DateTime(2022, 8, 7, 14, 0, 0);
            DateTime to3 = new DateTime(2022, 8, 7, 14, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from3, To = to3 }, userId);

            DateTime from4 = new DateTime(2022, 8, 7, 15, 0, 0);
            DateTime to4 = new DateTime(2022, 8, 7, 15, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from4, To = to4 }, userId);

            //act

            DateTime start = new DateTime(2022, 8, 7, 13, 30, 00);
            DateTime end = new DateTime(2022, 8, 7, 16, 30, 00);

            var result = (await _reservationService.FindAvailableTime(start, end, 25, 5)).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);

            Assert.AreEqual(firstAltarId, result[0].Key);
            Assert.AreEqual(4, result[0].Value.Count);

            var list = result[0].Value;

            Assert.AreEqual(firstAltarId, list[0].AltarId);
            Assert.AreEqual(to2, list[0].From);
            Assert.AreEqual(from3, list[0].To);

            Assert.AreEqual(firstAltarId, list[1].AltarId);
            Assert.AreEqual(to3, list[1].From);
            Assert.AreEqual(from4, list[1].To);

            Assert.AreEqual(firstAltarId, list[2].AltarId);
            Assert.AreEqual(to4, list[2].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[2].To);

            Assert.AreEqual(firstAltarId, list[3].AltarId);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[3].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 30, 0), list[3].To);
        }

        [TestMethod]
        public async Task ReservationsExistsForASpecificAltarHalfTheStartFrame_FindAvailableTimeForThatAltar_Success()
        {
            int firstAltarId = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });

            Guid userId = Guid.NewGuid();

            DateTime from1 = new DateTime(2022, 8, 7, 8, 0, 0);
            DateTime to1 = new DateTime(2022, 8, 7, 8, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from1, To = to1 }, userId);

            DateTime from2 = new DateTime(2022, 8, 7, 13, 0, 0);
            DateTime to2 = new DateTime(2022, 8, 7, 13, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from2, To = to2 }, userId);

            DateTime from3 = new DateTime(2022, 8, 7, 14, 0, 0);
            DateTime to3 = new DateTime(2022, 8, 7, 14, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from3, To = to3 }, userId);

            DateTime from4 = new DateTime(2022, 8, 7, 15, 0, 0);
            DateTime to4 = new DateTime(2022, 8, 7, 15, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from4, To = to4 }, userId);

            //act

            DateTime start = new DateTime(2022, 8, 7, 13, 10, 00);
            DateTime end = new DateTime(2022, 8, 7, 16, 30, 00);

            var result = (await _reservationService.FindAvailableTime(start, end, 25, 5)).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);

            Assert.AreEqual(firstAltarId, result[0].Key);
            Assert.AreEqual(4, result[0].Value.Count);

            var list = result[0].Value;

            Assert.AreEqual(firstAltarId, list[0].AltarId);
            Assert.AreEqual(to2, list[0].From);
            Assert.AreEqual(from3, list[0].To);

            Assert.AreEqual(firstAltarId, list[1].AltarId);
            Assert.AreEqual(to3, list[1].From);
            Assert.AreEqual(from4, list[1].To);

            Assert.AreEqual(firstAltarId, list[2].AltarId);
            Assert.AreEqual(to4, list[2].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[2].To);

            Assert.AreEqual(firstAltarId, list[3].AltarId);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[3].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 30, 0), list[3].To);
        }

        [TestMethod]
        public async Task ReservationsExistsForASpecificAltarHalfTheEndFrame_FindAvailableTimeForThatAltar_Success()
        {
            int firstAltarId = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });

            Guid userId = Guid.NewGuid();

            DateTime from1 = new DateTime(2022, 8, 7, 8, 0, 0);
            DateTime to1 = new DateTime(2022, 8, 7, 8, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from1, To = to1 }, userId);

            DateTime from2 = new DateTime(2022, 8, 7, 13, 0, 0);
            DateTime to2 = new DateTime(2022, 8, 7, 13, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from2, To = to2 }, userId);

            DateTime from3 = new DateTime(2022, 8, 7, 14, 0, 0);
            DateTime to3 = new DateTime(2022, 8, 7, 14, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from3, To = to3 }, userId);

            DateTime from4 = new DateTime(2022, 8, 7, 15, 0, 0);
            DateTime to4 = new DateTime(2022, 8, 7, 15, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from4, To = to4 }, userId);

            //act

            DateTime start = new DateTime(2022, 8, 7, 13, 30, 00);
            DateTime end = new DateTime(2022, 8, 7, 15, 15, 00);

            var result = (await _reservationService.FindAvailableTime(start, end, 25, 5)).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);

            Assert.AreEqual(firstAltarId, result[0].Key);
            Assert.AreEqual(2, result[0].Value.Count);

            var list = result[0].Value;

            Assert.AreEqual(firstAltarId, list[0].AltarId);
            Assert.AreEqual(to2, list[0].From);
            Assert.AreEqual(from3, list[0].To);

            Assert.AreEqual(firstAltarId, list[1].AltarId);
            Assert.AreEqual(to3, list[1].From);
            Assert.AreEqual(from4, list[1].To);
        }

        [TestMethod]
        public async Task TwoAltarsExistsAndReservationsExistsForASpecificAltar_FindAvailableTimeForThatAltar_Success()
        {
            int firstAltarId = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });
            int secondAltarId = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });

            Guid userId = Guid.NewGuid();

            DateTime from1 = new DateTime(2022, 8, 7, 8, 0, 0);
            DateTime to1 = new DateTime(2022, 8, 7, 8, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from1, To = to1 }, userId);

            DateTime from2 = new DateTime(2022, 8, 7, 13, 0, 0);
            DateTime to2 = new DateTime(2022, 8, 7, 13, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from2, To = to2 }, userId);

            DateTime from3 = new DateTime(2022, 8, 7, 14, 0, 0);
            DateTime to3 = new DateTime(2022, 8, 7, 14, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from3, To = to3 }, userId);

            DateTime from4 = new DateTime(2022, 8, 7, 15, 0, 0);
            DateTime to4 = new DateTime(2022, 8, 7, 15, 30, 0);
            await _reservationService.Reserve(KorbanTypes.Minha, new AvailableTimeDto() { AltarId = firstAltarId, From = from4, To = to4 }, userId);

            //act

            DateTime start = new DateTime(2022, 8, 7, 13, 10, 00);
            DateTime end = new DateTime(2022, 8, 7, 16, 30, 00);

            var result = (await _reservationService.FindAvailableTime(start, end, 25, 5)).ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual(firstAltarId, result[0].Key);
            Assert.AreEqual(4, result[0].Value.Count);

            var list = result[0].Value;

            Assert.AreEqual(firstAltarId, list[0].AltarId);
            Assert.AreEqual(to2, list[0].From);
            Assert.AreEqual(from3, list[0].To);

            Assert.AreEqual(firstAltarId, list[1].AltarId);
            Assert.AreEqual(to3, list[1].From);
            Assert.AreEqual(from4, list[1].To);

            Assert.AreEqual(firstAltarId, list[2].AltarId);
            Assert.AreEqual(to4, list[2].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[2].To);

            Assert.AreEqual(firstAltarId, list[3].AltarId);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 0, 0), list[3].From);
            Assert.AreEqual(new DateTime(2022, 8, 7, 16, 30, 0), list[3].To);

            Assert.AreEqual(secondAltarId, result[1].Key);
            Assert.AreEqual(6, result[1].Value.Count);

            list = result[1].Value;

            Assert.AreEqual(secondAltarId, list[0].AltarId);
            Assert.AreEqual(start, list[0].From);
            Assert.AreEqual(start.AddMinutes(30), list[0].To);

            Assert.AreEqual(secondAltarId, list[1].AltarId);
            Assert.AreEqual(start.AddMinutes(30), list[1].From);
            Assert.AreEqual(start.AddMinutes(60), list[1].To);

            Assert.AreEqual(secondAltarId, list[2].AltarId);
            Assert.AreEqual(start.AddMinutes(60), list[2].From);
            Assert.AreEqual(start.AddMinutes(90), list[2].To);

            Assert.AreEqual(secondAltarId, list[3].AltarId);
            Assert.AreEqual(start.AddMinutes(90), list[3].From);
            Assert.AreEqual(start.AddMinutes(120), list[3].To);

            Assert.AreEqual(secondAltarId, list[4].AltarId);
            Assert.AreEqual(start.AddMinutes(120), list[4].From);
            Assert.AreEqual(start.AddMinutes(150), list[4].To);

            Assert.AreEqual(secondAltarId, list[5].AltarId);
            Assert.AreEqual(start.AddMinutes(150), list[5].From);
            Assert.AreEqual(start.AddMinutes(180), list[5].To);
        }
    }
}

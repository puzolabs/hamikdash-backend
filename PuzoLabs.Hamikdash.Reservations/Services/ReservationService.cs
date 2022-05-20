using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using PuzoLabs.Hamikdash.Reservations.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Services
{
    public interface IReservationService
    {
        public Task<IEnumerable<AvailableTimeDto>> FindAvailableTime(string type, DateTime from, DateTime to, int workDuration, int restTime);
    }

    public class ReservationService : IReservationService
    {
        private readonly IAltarRepository _altarRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IAltarRepository altarRepository, IReservationRepository reservationRepository)
        {
            _altarRepository = altarRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<AvailableTimeDto>> FindAvailableTime(string type, DateTime from, DateTime to, int workDuration, int restTime)
        {
            SortedList<DateTime, AvailableTimeDto> result = new SortedList<DateTime, AvailableTimeDto>();

            IEnumerable<Altar> altars = await _altarRepository.GetAvailableAltars();

            foreach (Altar altar in altars)
            {
                //get from db, from future reservations table, the busy times for this altar
                Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(altar.Id, from, to)).ToArray();

                //find the available times between the busy times
                for (int i = 0; i < reservations.Length - 1; i++)
                {
                    Reservation currentReservation = reservations[i];
                    Reservation nextReservation = reservations[i + 1];

                    if ((nextReservation.From - currentReservation.To).Minutes > workDuration)
                    {
                        IEnumerable<AvailableTimeDto> timeSlots = FindAvailableTimeSlotsPerWorkDuration(currentReservation.To, nextReservation.From, workDuration, restTime);

                        foreach (var ts in timeSlots)
                            result.Add(ts.From, ts);
                    }
                }
            }

            return result.Values;
        }

        private IEnumerable<AvailableTimeDto> FindAvailableTimeSlotsPerWorkDuration(DateTime from, DateTime to, int workDuration, int restDuration)
        {
            return new List<AvailableTimeDto>()
            {
                new AvailableTimeDto()
                {
                    From = from,
                    To = to.AddMinutes(-restDuration),
                }
            };
        }
    }
}

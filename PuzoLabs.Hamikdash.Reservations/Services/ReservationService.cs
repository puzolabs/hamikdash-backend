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
        public Task<Dictionary<int, List<AvailableTimeDto>>> FindAvailableTime(DateTime from, DateTime to, int workDuration, int restTime);
        public Task Reserve(KorbanTypes korbanType, AvailableTimeDto timeDto, Guid userId);
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

        public async Task<Dictionary<int, List<AvailableTimeDto>>> FindAvailableTime(DateTime from, DateTime to, int workDuration, int restTime)
        {
            Dictionary<int, List<AvailableTimeDto>> allResults = new Dictionary<int, List<AvailableTimeDto>>();

            IEnumerable<Altar> altars = await _altarRepository.GetAvailableAltars();

            foreach (Altar altar in altars)
            {
                List<AvailableTimeDto> results = new List<AvailableTimeDto>();

                //get from db, from future reservations table, the busy times for this altar
                Reservation[] reservations = (await _reservationRepository.GetReservationsForAltarInTimeRangeOrderedAscending(altar.Id, from, to)).ToArray();

                if (reservations == null || reservations.Length == 0)
                {
                    FindAvailableTimeSlotsPerWorkDuration(from, to, workDuration, restTime, altar.Id, results);
                }
                else
                {
                    //find the available times before the busy times
                    if (reservations[0].StartDate > from)
                        FindAvailableTimeSlotsPerWorkDuration(from, reservations[0].StartDate, workDuration, restTime, altar.Id, results);

                    FindAvailableTimesBetweenTheBusyTimes(reservations, workDuration, restTime, altar.Id, results);

                    //find the available times after the busy times
                    if (reservations[^1].EndDate < to)
                        FindAvailableTimeSlotsPerWorkDuration(reservations[^1].EndDate, to, workDuration, restTime, altar.Id, results);
                }

                allResults.Add(altar.Id, results);
            }

            return allResults;
        }

        private void FindAvailableTimesBetweenTheBusyTimes(Reservation[] reservations, int workDuration, int restTime, int altarId, List<AvailableTimeDto> results)
        {
            for (int i = 0; i < reservations.Length - 1; i++)
            {
                Reservation currentReservation = reservations[i];
                Reservation nextReservation = reservations[i + 1];

                if ((nextReservation.StartDate - currentReservation.EndDate).Minutes >= workDuration + restTime)
                {
                    FindAvailableTimeSlotsPerWorkDuration(currentReservation.EndDate, nextReservation.StartDate, workDuration, restTime, altarId, results);
                }
            }
        }

        private void FindAvailableTimeSlotsPerWorkDuration(DateTime from, DateTime to, int workDuration, int restDuration, int altarId, List<AvailableTimeDto> results)
        {
            DateTime nextFrom = from, nextTo = nextFrom.AddMinutes(workDuration + restDuration);

            do
            {
                results.Add(new AvailableTimeDto()
                {
                    From = nextFrom,
                    To = nextTo,
                    AltarId = altarId
                });

                nextFrom = nextTo;
                nextTo = nextFrom.AddMinutes(workDuration + restDuration);
            } while (nextTo <= to);
        }

        public async Task Reserve(KorbanTypes korbanType, AvailableTimeDto timeDto, Guid userId)
        {
            Reservation reservation = new Reservation()
            {
                AltarId = timeDto.AltarId,
                StartDate = timeDto.From,
                EndDate = timeDto.To,
                Status = ReservationStatus.Pending,
                Type = korbanType,
                UserId = userId,
            };

            await _reservationRepository.Add(reservation);
        }
    }
}

using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Db
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to);
    }

    public class ReservationRepository : IReservationRepository
    {
        private readonly IDatabase _database;

        public ReservationRepository(IDatabase database)
        {
            _database = database;
        }

        public Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}

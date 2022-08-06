using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Db
{
    public interface IReservationRepository
    {
        Task<Guid> Add(Reservation reservation);
        Task<Reservation> Get(Guid id);
        Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to);
        Task DeleteAll();
    }

    public class ReservationRepository : IReservationRepository
    {
        private readonly IDatabase _database;
        private const string TABLE_NAME = "reservations";

        public ReservationRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<Guid> Add(Reservation reservation)
        {
            reservation.Id = Guid.NewGuid();

            string query = $"INSERT INTO {TABLE_NAME} (id, type, altarId, startDate, endDate, userId, status) VALUES ('{reservation.Id}', '{reservation.Type}', '{reservation.AltarId}', '{reservation.StartDate}', '{reservation.EndDate}', '{reservation.UserId}', '{reservation.Status}')";
            await _database.Do(query);
            
            return reservation.Id;
        }

        public async Task<Reservation> Get(Guid id)
        {
            string query = $"SELECT * FROM {TABLE_NAME} WHERE id = '{id}'";

            var reservations = await _database.QueryAsync<Reservation>(query);

            return reservations.FirstOrDefault();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to)
        {
            string query = @$"
                SELECT * FROM {TABLE_NAME}
                WHERE altarId = '{altarId}' and startDate >= '{from}' and endDate <= '{to}'
                order by startDate asc";

            var reservations = await _database.QueryAsync<Reservation>(query);

            return reservations;
        }

        public async Task DeleteAll()
        {
            string query = $"DELETE FROM {TABLE_NAME}";

            await _database.QueryAsync<Reservation>(query);
        }
    }
}

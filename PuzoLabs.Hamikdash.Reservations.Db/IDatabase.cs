using Microsoft.Extensions.Options;
using Npgsql;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace PuzoLabs.Hamikdash.Reservations.Db
{
    public interface IDatabase
    {
        Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to);
        Task<int> AddAltar(Altar altar);
        Task RemoveAllAltars();
        Task<IEnumerable<Altar>> GetAvailableAltars();
    }

    public class Database : IDatabase
    {
        private NpgsqlConnection _connection;
        private bool _opened = false;
        private readonly string _connectionString;

        public Database(IOptions<DbSettings> dbSettings)
        {
            _connectionString = dbSettings.Value.ToConnectionString();
        }

        #region Infra

        public async Task Open()
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();

            _opened = true;
        }

        public void Close()
        {
            _connection.Dispose();
            _opened = false;
        }

        private async Task Do(string query)
        {
            if (!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<T> DoAndReturn<T>(string query)
        {
            if (!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connection))
            {
                return (T)(await cmd.ExecuteScalarAsync());
            }
        }

        //private async Task Read(string query)
        //{
        //    if (!_opened)
        //        await Open();

        //    await using (var cmd = new NpgsqlCommand(query, _connction))
        //    await using (var reader = await cmd.ExecuteReaderAsync())
        //    {
        //        while (await reader.ReadAsync())
        //        {
        //            Console.WriteLine(reader.GetString(0));
        //        }
        //    }
        //}

        #endregion

        public async Task<IEnumerable<Altar>> GetAvailableAltars()
        {
            string query = "SELECT * FROM altars WHERE is_available = true";

            var altars = await _connection.QueryAsync<Altar>(query);

            return altars;
        }

        public async Task<int> AddAltar(Altar altar)
        {
            string query = $"INSERT INTO altars (is_available) VALUES ({altar.IsAvailable}) RETURNING {nameof(altar.Id)}";
            return await DoAndReturn<int>(query);
        }

        public async Task RemoveAllAltars()
        {
            string query = "DELETE FROM altars";

            await Do(query);
        }

        public Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(int altarId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}

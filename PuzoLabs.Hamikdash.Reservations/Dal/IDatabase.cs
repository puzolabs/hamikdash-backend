using Microsoft.Extensions.Options;
using Npgsql;
using PuzoLabs.Hamikdash.Reservations.Dal.Models;
using PuzoLabs.Hamikdash.Reservations.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Dal
{
    public interface IDatabase
    {
        Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(Guid altarId, DateTime from, DateTime to);
        Task<int> AddAltar(Altar altar);
        Task<IEnumerable<Altar>> GetAvailableAltars();
    }

    public class Database : IDatabase
    {
        private NpgsqlConnection _connction;
        private bool _opened = false;
        private readonly string _connectionString;

        public Database(IOptions<DbSettings> dbSettings)
        {
            _connectionString = dbSettings.Value.ToConnectionString();
        }

        public async Task Open()
        {
            //var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

            _connction = new NpgsqlConnection(_connectionString);
            await _connction.OpenAsync();

            _opened = true;
        }

        public void Close()
        {
            _connction.Dispose();
            _opened = false;
        }

        private async Task Do(string query)
        {
            if(!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connction))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<T> DoAndReturn<T>(string query)
        {
            if (!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connction))
            {
                return (T)(await cmd.ExecuteScalarAsync());
            }
        }

        private async Task Read(string query)
        {
            if (!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connction))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
        }

        public Task<IEnumerable<Altar>> GetAvailableAltars()
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAltar(Altar altar)
        {
            string query = $"INSERT INTO altars (is_available) VALUES ({altar.IsAvailable}) RETURNING {nameof(altar.Id)}";
            return await DoAndReturn<int>(query);
        }

        public Task<IEnumerable<Reservation>> GetReservationsForAltarInTimeRangeOrderedAscending(Guid altarId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}

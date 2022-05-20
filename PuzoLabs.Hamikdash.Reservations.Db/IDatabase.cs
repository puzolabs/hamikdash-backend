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
        Task Do(string query);
        Task<T> DoAndReturn<T>(string query);
        Task<IEnumerable<T>> QueryAsync<T>(string query);
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

        public async Task Do(string query)
        {
            if (!_opened)
                await Open();

            await using (var cmd = new NpgsqlCommand(query, _connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<T> DoAndReturn<T>(string query)
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

        public async Task<IEnumerable<T>> QueryAsync<T>(string query)
        {
            return await _connection.QueryAsync<T>(query);
        }
    }
}

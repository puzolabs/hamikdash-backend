using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Db
{
    public interface IAltarRepository
    {
        Task<int> AddAltar(Altar altar);
        Task RemoveAllAltars();
        Task<IEnumerable<Altar>> GetAvailableAltars();
    }

    public class AltarRepository : IAltarRepository
    {
        private readonly IDatabase _database;

        public AltarRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<Altar>> GetAvailableAltars()
        {
            string query = "SELECT * FROM altars WHERE is_available = true";

            var altars = await _database.QueryAsync<Altar>(query);

            return altars;
        }

        public async Task<int> AddAltar(Altar altar)
        {
            string query = $"INSERT INTO altars (is_available) VALUES ({altar.IsAvailable}) RETURNING {nameof(altar.Id)}";
            return await _database.DoAndReturn<int>(query);
        }

        public async Task RemoveAllAltars()
        {
            string query = "DELETE FROM altars";

            await _database.Do(query);
        }
    }
}

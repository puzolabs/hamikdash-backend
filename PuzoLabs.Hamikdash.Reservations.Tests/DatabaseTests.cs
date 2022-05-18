using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        IOptions<DbSettings> _options;

        [TestInitialize]
        public async Task InitConfiguration()
        {
            InitManager initManager = new InitManager();
            await initManager.Init();
            _options = initManager.DbOptions;
        }

        [TestMethod]
        public async Task TestAddAltar()
        {
            IDatabase database = new Database(_options);
            int id = await database.AddAltar(new Altar() { IsAvailable = true });
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public async Task TestGetAvailableAltars()
        {
            IDatabase database = new Database(_options);

            await database.RemoveAllAltars();
            await database.AddAltar(new Altar() { IsAvailable = true });
            await database.AddAltar(new Altar() { IsAvailable = false });
            await database.AddAltar(new Altar() { IsAvailable = true });
            var altars = await database.GetAvailableAltars();
            
            Assert.AreEqual(2, altars.ToArray().Length);
        }
    }
}

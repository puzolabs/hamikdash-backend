using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Db.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class AltarRepositoryTests
    {
        IAltarRepository _altarRepository;

        [TestInitialize]
        public async Task InitConfiguration()
        {
            InitManager initManager = new InitManager();
            await initManager.Init();
            _altarRepository = initManager.AltarRepository;
        }

        [TestMethod]
        public async Task TestAddAltar()
        {
            int id = await _altarRepository.AddAltar(new Altar() { IsAvailable = true });
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public async Task TestGetAvailableAltars()
        {
            await _altarRepository.RemoveAllAltars();
            await _altarRepository.AddAltar(new Altar() { IsAvailable = true });
            await _altarRepository.AddAltar(new Altar() { IsAvailable = false });
            await _altarRepository.AddAltar(new Altar() { IsAvailable = true });
            var altars = await _altarRepository.GetAvailableAltars();
            
            Assert.AreEqual(2, altars.ToArray().Length);
        }
    }
}

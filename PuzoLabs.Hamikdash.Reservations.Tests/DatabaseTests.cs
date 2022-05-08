using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzoLabs.Hamikdash.Reservations.Dal;
using PuzoLabs.Hamikdash.Reservations.Dal.Models;
using PuzoLabs.Hamikdash.Reservations.Db;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        private IConfigurationRoot _config;
        IOptions<DbSettings> _options;

        [TestInitialize]
        public async Task InitConfiguration()
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            services.AddLogging(logging => logging.AddConsole());

            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.test.json")
                //.AddEnvironmentVariables()
                .Build();

            services.Configure<DbSettings>(_config.GetSection($"{DbSettings.SettingsKeyName}"));
            services.AddSingleton<IHostedService, DatabaseMigrator>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            _options = serviceProvider.GetRequiredService<IOptions<DbSettings>>();

            //migrate db
            var migrator = serviceProvider.GetRequiredService<IHostedService>();
            //IHostedService migrator = new DatabaseMigrator(_options, null);
            await migrator.StartAsync(new CancellationToken());
        }

        [TestMethod]
        public async Task TestAltar()
        {
            //var options = _config.Get<DbSettings>();
            //var options = _config.GetSection($"{DbSettings.SettingsKeyName}").Get<DbSettings>();
            IDatabase database = new Database(_options);
            int id = await database.AddAltar(new Altar() { IsAvailable = true });
            Assert.IsNotNull(id);
        }
    }
}

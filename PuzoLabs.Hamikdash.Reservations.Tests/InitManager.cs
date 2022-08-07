using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using PuzoLabs.Hamikdash.Reservations.Db;
using PuzoLabs.Hamikdash.Reservations.Services;

namespace PuzoLabs.Hamikdash.Reservations.Tests
{
    public class InitManager
    {
        public IAltarRepository AltarRepository { get; private set; }
        public IReservationRepository ReservationRepository { get; private set; }
        public IReservationService ReservationService { get; set; }

        public async Task Init()
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            services.AddLogging(logging => logging.AddConsole());

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.test.json")
                //.AddEnvironmentVariables()
                .Build();

            services.Configure<DbSettings>(config.GetSection($"{DbSettings.SettingsKeyName}"));
            services.AddSingleton<IHostedService, DatabaseMigrator>();
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<IAltarRepository, AltarRepository>();
            services.AddSingleton<IReservationRepository, ReservationRepository>();

            services.AddSingleton<IReservationService, ReservationService>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            AltarRepository = serviceProvider.GetRequiredService<IAltarRepository>();
            ReservationRepository = serviceProvider.GetRequiredService<IReservationRepository>();

            //migrate db
            var migrator = serviceProvider.GetRequiredService<IHostedService>();
            await migrator.StartAsync(new CancellationToken());

            ReservationService = serviceProvider.GetRequiredService<IReservationService>();
        }
    }
}

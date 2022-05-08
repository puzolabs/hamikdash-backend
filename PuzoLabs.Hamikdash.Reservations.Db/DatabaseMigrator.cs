using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PuzoLabs.Hamikdash.Reservations.Db
{
    public class DatabaseMigrator : IHostedService
    {
        private readonly string m_ConnectionString;

        private readonly ILogger<DatabaseMigrator> m_Logger;

        public DatabaseMigrator(IOptions<DbSettings> dbSettings, ILoggerFactory loggerFactory)
        {
            m_ConnectionString = dbSettings.Value.ToConnectionString();

            m_Logger = loggerFactory.CreateLogger<DatabaseMigrator>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            EnsureDatabase.For.PostgresqlDatabase(m_ConnectionString);

            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(m_ConnectionString)
                .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .WithVariablesDisabled()
                .LogToConsole()
                .Build();

            if (upgradeEngine.IsUpgradeRequired())
            {
                var result = upgradeEngine.PerformUpgrade();

                if (!result.Successful)
                {
                    var errorMessage = $"Migration script failed: {result.ErrorScript}";

                    m_Logger.LogCritical(errorMessage, result.Error);
                    throw new Exception(errorMessage, result.Error);
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

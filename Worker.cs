using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerServiceCS
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
                
                var provider = new AzureServiceTokenProvider();
                var kv = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(provider.KeyVaultTokenCallback));

                var secret2 = await kv
                    .GetSecretAsync("https://xxx-xxxxx-key-vault.vault.azure.net", "MongoDBConnectionString")
                    .ConfigureAwait(false);
                _logger.LogInformation("Data2 is: {data}", secret2);
            }
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }        
    }
}

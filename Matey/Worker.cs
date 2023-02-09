namespace Matey
{
    using ConfigurationSource.Abstractions;

    public class Worker : BackgroundService
    {
        private readonly IServiceBroker broker;
        private readonly IConfigurationSource backend;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceBroker broker, IConfigurationSource backend, ILogger<Worker> logger)
        {
            this.broker = broker;
            this.backend = backend;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            broker.Synchronize();
            await backend.BeginMonitorAsync(stoppingToken);
        }
    }
}
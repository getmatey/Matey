namespace Matey
{
    using Backend.Abstractions;

    public class Worker : BackgroundService
    {
        private readonly IServiceBroker broker;
        private readonly IBackend backend;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceBroker broker, IBackend backend, ILogger<Worker> logger)
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
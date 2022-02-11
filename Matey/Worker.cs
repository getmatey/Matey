namespace Matey
{
    using Backend.Abstractions;

    public class Worker : BackgroundService
    {
        private readonly IBackend backend;
        private readonly ILogger<Worker> _logger;

        public Worker(IBackend backend, ILogger<Worker> logger)
        {
            this.backend = backend;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await backend.BeginMonitorAsync(stoppingToken);
        }
    }
}
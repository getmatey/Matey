using Docker.DotNet;
using Docker.DotNet.Models;
using Matey.Common;
using Microsoft.Extensions.Logging;

namespace Matey.Backend.Docker
{
    public class DockerBackend : IBackend
    {
        private readonly DockerClient client;
        private readonly INotifier notifier;
        private readonly ILogger<DockerBackend> logger;

        public DockerBackend(DockerClient client, INotifier notifier, ILogger<DockerBackend> logger)
        {
            this.client = client;
            this.notifier = notifier;
            this.logger = logger;
        }

        public async Task BeginMonitorAsync(CancellationToken cancellationToken)
        {
            // Filter for stop and start events
            IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>
            {
                {
                    "event",
                    new Dictionary<string, bool>
                    {
                        { "start", true },
                        { "stop", true },
                        { "die", true }
                    }
                }
            };

            // Hook for docker events
            Progress<Message> progress = new Progress<Message>();
            progress.ProgressChanged += OnProgressChanged;

            // Begin monitoring
            await client.System.MonitorEventsAsync(new ContainerEventsParameters() { Filters = filters }, progress, cancellationToken);
        }

        private void OnProgressChanged(object? sender, Message e)
        {
            logger.LogInformation("Status: {0}, from: {1}, scope: {2}", e.Status, e.From, e.Scope);
        }
    }
}

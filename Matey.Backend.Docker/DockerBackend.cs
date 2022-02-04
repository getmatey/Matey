using Docker.DotNet;
using Docker.DotNet.Models;
using Matey.Backend.Docker.Attributes;
using Matey.Common;
using Matey.Frontend;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Matey.Backend.Docker
{
    public class DockerBackend : IBackend
    {
        private readonly IOptions<DockerOptions> options;
        private readonly DockerClient client;
        private readonly INotifier notifier;
        private readonly ILogger<DockerBackend> logger;

        public DockerBackend(
            IOptions<DockerOptions> options,
            DockerClient client,
            INotifier notifier,
            ILogger<DockerBackend> logger)
        {
            this.options = options;
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
            IAttributeRoot attributes = new AttributeRoot(options.Value?.LabelPrefix ?? Defaults.LABEL_PREFIX, e.Actor.Attributes);
            IServiceConfiguration configuration = new DockerServiceConfiguration(attributes);
            logger.LogInformation("ID: {0}, Enabled: {1}", e.ID, configuration.IsEnabled);
            foreach(var backend in configuration.Backends)
            {
                logger.LogInformation("Backend: {0}, Port: {1}, Frontend rule: {2}", backend.Name, backend.Port, backend.Frontend.Rule);
            }
        }
    }
}

using Docker.DotNet;
using Docker.DotNet.Models;
using Matey.Backend.Docker.Attributes;
using Matey.Common;
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
            IBackendServiceConfiguration configuration = new DockerBackendServiceConfiguration(attributes);
            logger.LogInformation("Port: {0}\nEnabled: {1}", configuration.Port, configuration.IsEnabled);
        }
    }
}

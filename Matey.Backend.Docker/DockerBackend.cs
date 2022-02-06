using Docker.DotNet;
using Docker.DotNet.Models;
using Matey.Backend.Docker.Attributes;
using Matey.Common;
using Matey.Frontend;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

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
                        { DockerEvent.Start, true },
                        { DockerEvent.Stop, true },
                        { DockerEvent.Die, true }
                    }
                }
            };

            // Hook for docker events
            Progress<Message> progress = new Progress<Message>();
            progress.ProgressChanged += OnContainerLifecycleEvent;

            // Begin monitoring
            await client.System.MonitorEventsAsync(new ContainerEventsParameters() { Filters = filters }, progress, cancellationToken);
        }

        private void OnContainerLifecycleEvent(object? sender, Message e)
        {
            switch (e.Action)
            {
                case DockerEvent.Start:
                    OnContainerStart(sender, e);
                    break;

                case DockerEvent.Stop:
                case DockerEvent.Die:
                    OnContainerStop(sender, e);
                    break;
            }
        }

        private void OnContainerStart(object? sender, Message e)
        {
            // Filter for identifier
            IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>
            {
                {
                    "id",
                    new Dictionary<string, bool>
                    {
                        { e.ID, true }
                    }
                }
            };

            client.Containers.ListContainersAsync(new ContainersListParameters { Filters = filters })
                .ContinueWith(t =>
                {
                    if (!t.IsFaulted && !t.IsCanceled)
                    {
                        ContainerListResponse container = t.Result.First();
                        // TODO: Configurable network.
                        EndpointSettings endpointSettings = container.NetworkSettings.Networks.First().Value;
                        IAttributeRoot attributes = new AttributeRoot(options.Value?.LabelPrefix ?? Defaults.LABEL_PREFIX, e.Actor.Attributes);
                        IServiceConfiguration configuration = DockerServiceConfigurationFactory.Create(
                            attributes,
                            a => DockerBackendServiceConfigurationFactory.Create(a, IPAddress.Parse(endpointSettings.IPAddress)));
                        logger.LogInformation("ID: {0}, Enabled: {1}", e.ID, configuration.IsEnabled);
                        foreach (var backend in configuration.Backends)
                        {
                            logger.LogInformation("Backend: {0}, IP Address: {1}, Port: {2}, Frontend rule: {3}", backend.Name, backend.IPAddress, backend.Port, backend.Frontend.Rule);
                        }
                    }
                });
        }

        private void OnContainerStop(object? sender, Message e)
        {

        }
    }
}

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
            Task? task = null;
            switch (e.Action)
            {
                case DockerEvent.Start:
                    task = OnContainerStartAsync(sender, e);
                    break;

                case DockerEvent.Stop:
                case DockerEvent.Die:
                    task = OnContainerStopAsync(sender, e);
                    break;
            }

            // Log uncaught task exceptions.
            if(task is not null)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception is not null)
                    {
                        foreach (Exception ex in t.Exception.InnerExceptions)
                        {
                            logger.LogError(ex, "An error occurred while handing a container event.");
                        }
                    }
                });
            }
        }

        private async Task OnContainerStartAsync(object? sender, Message e)
        {
            // Filter by the container identifier
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

            // Find the container which caused the start event.
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters { Filters = filters });
            ContainerListResponse container = containers.First();
            
            // TODO: Configurable network.
            EndpointSettings endpointSettings = container.NetworkSettings.Networks.First().Value;

            // Build a service configuration from the container attributes.
            IAttributeRoot attributes = new AttributeRoot(options.Value?.LabelPrefix ?? ConfigurationDefault.LabelPrefix, e.Actor.Attributes);
            IServiceConfiguration configuration = DockerServiceConfigurationFactory.Create(
                attributes,
                a => DockerBackendServiceConfigurationFactory.Create(a, IPAddress.Parse(endpointSettings.IPAddress)));
            
            // Notify listeners that the service is online.
            await notifier.NotifyAsync(new ServiceOnlineNotification(configuration));
        }

        private Task OnContainerStopAsync(object? sender, Message e)
        {
            return Task.CompletedTask;
        }
    }
}

﻿using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Matey.Backend.Docker
{
    using Abstractions;
    using Attributes;
    using Common;

    public class DockerBackend : IBackend
    {
        private readonly IOptions<DockerOptions> options;
        private readonly DockerClient client;
        private readonly INotifier notifier;
        private readonly ILogger<DockerBackend> logger;
        internal const string ProviderName = "docker";

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

        private string GetLabelPrefix()
        {
            return options.Value?.LabelPrefix ?? DockerOptionsDefaults.LabelPrefix;
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
            string containerName = e.Actor.Attributes["name"];

            logger.LogDebug("Event '{0}' on container '{1}'", e.Action, containerName);
            switch (e.Action)
            {
                case DockerEvent.Start:
                    task = OnContainerStartAsync(sender, e);
                    break;

                case DockerEvent.Die:
                    task = OnContainerStopAsync(sender, e);
                    break;
            }

            // Log uncaught task exceptions.
            if (task is not null)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception is not null)
                    {
                        foreach (Exception ex in t.Exception.InnerExceptions)
                        {
                            logger.LogError(ex, "An error occurred while handing '{0}' event on container '{1}'.", e.Action, containerName);
                        }
                    }
                });
            }
        }

        private DockerServiceConfiguration CreateServiceConfiguration(ContainerListResponse container)
        {
            // Build a service configuration from the container attributes.
            IAttributeRoot attributes = new AttributeRoot(GetLabelPrefix(), container.Labels);

            return DockerServiceConfigurationFactory.Create(
                attributes,
                container.ID.Substring(0, 12), // Truncated container identifier
                container.Names.First().Replace("/", ""),
                attr => DockerBackendServiceConfigurationFactory.Create(
                    attr,
                    container.NetworkSettings.Networks,
                    DockerFrontendServiceConfigurationFactory.Create,
                    attr => DockerLoadBalancerConfigurationFactory.Create(attr, DockerLoadBalancerStickinesssConfigurationFactory.Create)));
        }

        private async Task<IServiceConfiguration> CreateServiceConfigurationAsync(Message e)
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

            return CreateServiceConfiguration(container);
        }

        private async Task OnContainerStartAsync(object? sender, Message e)
        {
            IServiceConfiguration configuration = await CreateServiceConfigurationAsync(e);

            // Notify listeners that the service is online.
            await notifier.NotifyAsync(new ServiceOnlineNotification(configuration));
        }

        private async Task OnContainerStopAsync(object? sender, Message e)
        {
            IServiceConfiguration configuration = await CreateServiceConfigurationAsync(e);

            // Notify listeners that the service is offline.
            await notifier.NotifyAsync(new ServiceOfflineNotification(configuration));
        }

        public IEnumerable<IServiceConfiguration> GetRunningServiceConfigurations()
        {
            IList<ContainerListResponse> containers = client.Containers
                .ListContainersAsync(new ContainersListParameters() { All = true })
                .GetAwaiter()
                .GetResult();

            foreach (ContainerListResponse container in containers.Where(c => c.State == "running" && c.Labels.Any(l => l.Key.StartsWith(GetLabelPrefix()))))
            {
                IAttributeRoot attributes = new AttributeRoot(GetLabelPrefix(), container.Labels);
                bool? isEnabled;
                if (!attributes.TryGetValue<bool>(Tokens.Enabled, out isEnabled) || (isEnabled ?? true))
                {
                    yield return CreateServiceConfiguration(container);
                }
            }
        }
    }
}

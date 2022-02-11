using Matey;
using Matey.Backend.Abstractions;
using Matey.Backend.Docker;
using Matey.Common;
using Matey.Common.Microsoft.DependencyInjection;
using Matey.Frontend.IIS;
using MediatR;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        services.AddMediatR(typeof(Worker).Assembly);
        services.AddTransient<INotifier, MediatorNotifierAdapter>();
        services.ConfigureDockerBackend(options => configuration.Bind("Docker", options));
        services.ConfigureIISFrontend(options => configuration.Bind("IIS", options));
        services.AddTransient<ServiceBroker>();
        services.AddNotificationHandler<ServiceOnlineNotification>(sp => sp.GetRequiredService<ServiceBroker>());
        services.AddNotificationHandler<ServiceOfflineNotification>(sp => sp.GetRequiredService<ServiceBroker>());
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

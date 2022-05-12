using Matey;
using Matey.Backend.Abstractions;
using Matey.Backend.Docker;
using Matey.Common;
using Matey.Common.Microsoft.DependencyInjection;
using Matey.Frontend.IIS;
using Matey.Rules;
using MediatR;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        services.AddMediatR(typeof(Worker).Assembly);
        services.AddTransient<INotifier, MediatorNotifierAdapter>();
        services.ConfigureDockerBackend(options => configuration.Bind("Docker", options));
        services.ConfigureIISFrontend(options => configuration.Bind("IIS", options));
        services.AddTransient<IRequestRuleParser, RequestRuleParser>();
        services.AddTransient<IServiceBroker, ServiceBroker>();
        services.AddNotificationHandler<ServiceOnlineNotification>(sp => sp.GetRequiredService<IServiceBroker>());
        services.AddNotificationHandler<ServiceOfflineNotification>(sp => sp.GetRequiredService<IServiceBroker>());
        services.AddHostedService<Worker>();
    })
    .UseWindowsService(c => c.ServiceName = "Matey Configurator Service")
    .Build();

await host.RunAsync();

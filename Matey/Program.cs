using Matey;
using Matey.Common;
using Matey.Common.Microsoft.DependencyInjection;
using Matey.ConfigurationSource.Abstractions;
using Matey.ConfigurationSource.Docker;
using Matey.Rules;
using Matey.WebServer.IIS;
using MediatR;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        string serviceName = context.HostingEnvironment.ApplicationName.ToLower();
        string environment = context.HostingEnvironment.EnvironmentName;
        
        configuration
            .AddJsonFile($"{serviceName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"{serviceName}.{environment}.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"{serviceName}.local.json", optional: true, reloadOnChange: false);
    }).ConfigureServices((context, services) =>
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

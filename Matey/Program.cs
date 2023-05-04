using Matey;
using Matey.Acme;
using Matey.Common;
using Matey.Common.Microsoft.DependencyInjection;
using Matey.ConfigurationSource.Abstractions;
using Matey.ConfigurationSource.Docker;
using Matey.Pki;
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
        services.AddDockerConfigurationSource(options => configuration.Bind("Docker", options));
        services.AddIISWebServer(options => configuration.Bind("IIS", options));
        services.AddAcmeChallengeResponder(options => configuration.Bind("PKI:ACME", options));
        services.AddCertificateStore(options => configuration.Bind("PKI:CertificateStore"));
        services.AddTransient<IRequestRuleParser, RequestRuleParser>();
        services.AddTransient<IServiceBroker, ServiceBroker>();
        services.AddHostedService<Worker>();
    })
    .UseWindowsService(c => c.ServiceName = "Matey Configurator Service")
    .Build();

await host.RunAsync();

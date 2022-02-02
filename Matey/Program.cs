using Matey;
using Matey.Backend.Docker.Microsoft.DependencyInjection;
using Matey.Common;
using MediatR;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        services.AddMediatR(typeof(Worker).Assembly);
        services.AddTransient<INotifier, MediatorNotifierAdapter>();
        services.ConfigureDockerBackend(options => configuration.Bind("Docker", options));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

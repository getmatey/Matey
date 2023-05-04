using ACMESharp.Protocol;
using Matey.Acme.Http01;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Matey.Acme
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddAcmeChallengeResponder(
            this IServiceCollection services,
            Action<AcmeOptions> configureOptions)
        {
            services.AddOptions<AcmeOptions>().Configure(configureOptions);

            services.AddSingleton(sp =>
            {
                IOptions<AcmeOptions>? options = sp.GetService<IOptions<AcmeOptions>>();
                AcmeEnvironmentOptions? environment = options?.Value?.GetActiveEnvironment();

                if (environment?.CertificateAuthorityUri != null)
                {
                    return new AcmeProtocolClient(new Uri(environment.CertificateAuthorityUri), usePostAsGet: environment.UsePostAsGet);
                }
                else
                {
                    return null;
                }
            });
            services.AddSingleton<Http01ChallengeValidationDetailsRepository>()
                .AddSingleton<IChallengeValidationDetailsRepository, Http01ForwardingChallengeValidationDetailsRepository>();
            services.AddSingleton<AcmeHttp01ChallengeResponderHostedService>()
                .AddHostedService(sp => sp.GetRequiredService<AcmeHttp01ChallengeResponderHostedService>());
            services.AddTransient<IAcmeCertificateIssuer, AcmeCertificateIssuer>();

            return services;
        }
    }
}
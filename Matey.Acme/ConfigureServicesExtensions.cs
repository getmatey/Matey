using ACMESharp.Protocol;
using ACMESharp.Protocol.Resources;
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
            services.AddOptions<AcmeOptions>().Configure(configureOptions).PostConfigure(options =>
            {
                AcmeEnvironmentOptions environment = options.GetActiveEnvironment();

                if (environment.IsStaging())
                {
                    services.AddSingleton<StagingCertificateStore>();
                }
                else
                {
                    throw new NotSupportedException();
                }

                services.AddSingleton(sp => new AcmeProtocolClient(new Uri(environment.CertificateAuthorityUri)));
            });
            services.AddTransient<IChallengeValidationDetailsRepository, MemoryChallengeValidationDetailsRepository>();
            services.AddHostedService<AcmeHttp01ChallengeResponderHostedService>();

            return services;
        }
    }
}
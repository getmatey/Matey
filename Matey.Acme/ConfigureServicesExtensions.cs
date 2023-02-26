using Matey.Acme.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Matey.Acme
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddAcmeChallengeResponder(
            this IServiceCollection services,
            Action<AcmeOptions> configureOptions)
        {
            services.AddOptions<AcmeOptions>().Configure(configureOptions);
            services.AddTransient<IChallengeValidationDetailsRepository, MemoryChallengeValidationDetailsRepository>();
            services.AddHostedService<AcmeHttpChallengeResponderHostedService>();

            return services;
        }
    }
}
using Matey.Pki.X509Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Matey.Pki
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddCertificateStore(this IServiceCollection services, Action<X509StoreCertificateStoreOptions> configureOptions)
        {
            services.AddOptions<X509StoreCertificateStoreOptions>()
                .Configure(configureOptions);

            services.AddTransient<ICertificateStore>(sp =>
            {
                IOptions<X509StoreCertificateStoreOptions>? options = sp.GetService<IOptions<X509StoreCertificateStoreOptions>>();

                if (options != null)
                {
                    return new X509StoreCertificateStore(options);
                }
                else
                {
                    return null;
                }
            });

            return services;
        }
    }
}

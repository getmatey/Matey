using Microsoft.Extensions.Options;
using X509Certificates = System.Security.Cryptography.X509Certificates;

namespace Matey.Pki.X509Store
{
    internal class X509StoreCertificateStore : ICertificateStore, IDisposable
    {
        private readonly X509StoreCertificateStoreOptions options;
        private readonly X509Certificates.X509Store x509Store;

        public X509StoreCertificateStore(IOptions<X509StoreCertificateStoreOptions> options)
        {
            this.options = options.Value;
            X509Certificates.StoreLocation storeLocation = Enum.Parse<X509Certificates.StoreLocation>(this.options.StoreLocation);
            x509Store = new X509Certificates.X509Store(this.options.StoreName, storeLocation);
        }

        public void Dispose()
        {
            x509Store.Dispose();
        }

        public Task InstallCertificateAsync(X509Certificates.X509Certificate2 certificate)
        {
            if (!x509Store.IsOpen)
            {
                x509Store.Open(X509Certificates.OpenFlags.ReadWrite);
            }

            x509Store.Certificates.Add(certificate);

            return Task.CompletedTask;
        }
    }
}

using System.Security.Cryptography.X509Certificates;

namespace Matey.Pki
{
    public interface ICertificateStore
    {
        Task InstallCertificateAsync(X509Certificate2 certificate);
    }
}
using System.Security.Cryptography.X509Certificates;

namespace Matey.Acme
{
    public interface IAcmeCertificateIssuer
    {
        Task<X509Certificate2> OrderCertificateAsync(params string[] domainNames);
    }
}

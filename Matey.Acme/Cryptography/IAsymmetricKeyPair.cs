using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography;

namespace Matey.Acme.Cryptography
{
    public interface IAsymmetricKeyPair
    {
        Pkcs10CertificationRequest CreateCertificationRequest(IEnumerable<string> names, HashAlgorithmName? hashAlgor = null);
    }
}

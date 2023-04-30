using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using SysHashAlgorithName = System.Security.Cryptography.HashAlgorithmName;
using System.Collections;

namespace Matey.Acme.Cryptography
{
    public class RsaAsymmetricKeyPair : IAsymmetricKeyPair
    {
        private const int RSA_BITS_DEFAULT = 2048;
        private const int RSA_BITS_MINIMUM = 1024 + 1; // LE no longer allows 1024-bit PrvKeys
        
        private static readonly BigInteger RSA_E_3 = BigInteger.Three;
        private static readonly BigInteger RSA_E_F4 = BigInteger.ValueOf(0x10001);

        // This is based on the BC RSA Generator code:
        //    https://github.com/bcgit/bc-csharp/blob/fba5af528ce7dcd0ac0513363196a62639b82a86/crypto/src/crypto/generators/RsaKeyPairGenerator.cs#L37
        private const int DEFAULT_CERTAINTY = 100;

        private readonly AsymmetricCipherKeyPair ackp;

        private RsaAsymmetricKeyPair(AsymmetricCipherKeyPair ackp)
        {
            this.ackp = ackp ?? throw new ArgumentNullException(nameof(ackp));
        }

        public Pkcs10CertificationRequest CreateCertificationRequest(
            IEnumerable<string> names,
            SysHashAlgorithName? hashAlgor = null)
        {
            if (hashAlgor == null)
            {
                hashAlgor = SysHashAlgorithName.SHA256;
            }

            Dictionary<DerObjectIdentifier, string> attrs = new()
            {
                [X509Name.CN] = names.First(),
            };
            X509Name subj = new X509Name(attrs.Keys.ToList(), attrs.Values.ToList());

            string sigAlg = $"{hashAlgor.Value.Name}withRSA";
            IList<Asn1Encodable> csrAttrs = new List<Asn1Encodable>();

            IList<GeneralName> gnames = new List<GeneralName>(
                    names.Select(x => new GeneralName(GeneralName.DnsName, x)));

            GeneralNames altNames = new GeneralNames(gnames.ToArray());
            #pragma warning disable CS0612 // Type or member is obsolete
            X509Extensions x509Ext = new X509Extensions(new Hashtable
            {
                [X509Extensions.SubjectAlternativeName] = new X509Extension(
                        false, new DerOctetString(altNames))
            });
            #pragma warning restore CS0612 // Type or member is obsolete

            csrAttrs.Add(new Org.BouncyCastle.Asn1.Cms.Attribute(
                    PkcsObjectIdentifiers.Pkcs9AtExtensionRequest,
                    new DerSet(x509Ext)));

            #pragma warning disable CS0618 // Type or member is obsolete
            return new Pkcs10CertificationRequest(sigAlg,
                    subj, ackp.Public, new DerSet(csrAttrs.ToArray()), ackp.Private);
            #pragma warning restore CS0618 // Type or member is obsolete
        }

        public static RsaAsymmetricKeyPair Generate(int bits, string? PubExp = null)
        {
            if (bits < RSA_BITS_MINIMUM)
                bits = RSA_BITS_DEFAULT;

            BigInteger e;
            if (string.IsNullOrEmpty(PubExp))
                e = RSA_E_F4;
            else if (PubExp.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                e = new BigInteger(PubExp, 16);
            else
                e = new BigInteger(PubExp);

            RsaKeyGenerationParameters rsaKgp = new RsaKeyGenerationParameters(
                    e, new SecureRandom(), bits, DEFAULT_CERTAINTY);
            RsaKeyPairGenerator rkpg = new RsaKeyPairGenerator();
            rkpg.Init(rsaKgp);
            return new RsaAsymmetricKeyPair(rkpg.GenerateKeyPair());
        }
    }
}

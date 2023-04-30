using ACMESharp.Authorizations;
using ACMESharp.Protocol;
using ACMESharp.Protocol.Resources;
using Matey.Acme.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Matey.Acme
{
    internal class AcmeCertificateIssuer : IAcmeCertificateIssuer
    {
        private readonly ILogger<AcmeCertificateIssuer> logger;
        private readonly AcmeOptions options;
        private readonly AcmeProtocolClient acme;
        private readonly IChallengeValidationDetailsRepository challengeValidationRepository;

        public AcmeCertificateIssuer(
            ILogger<AcmeCertificateIssuer> logger,
            IOptions<AcmeOptions> options,
            AcmeProtocolClient acme,
            IChallengeValidationDetailsRepository challengeValidationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.acme = acme ?? throw new ArgumentNullException(nameof(acme));
            this.challengeValidationRepository = challengeValidationRepository ?? throw new ArgumentNullException(nameof(challengeValidationRepository));
        }

        public async Task<X509Certificate2> OrderCertificateAsync(params string[] domainNames)
        {
            acme.Directory = await acme.GetDirectoryAsync();
            await acme.GetNonceAsync();

            acme.Account = await acme.CreateAccountAsync(options.AccountContactEmails, options.AcceptTermsOfService);

            OrderDetails order = await acme.CreateOrderAsync(domainNames);
            logger.LogInformation("Certificate order '{0}' created for the domain names: {1}.", order.OrderUrl, string.Join(", ", domainNames));

            order = await AuthorizeOrderAsync(order);
            logger.LogInformation("Certificate order '{0}' has been authorized.", order.OrderUrl);

            if (order.Payload.Status == "ready")
            {
                logger.LogInformation("Certificate order '{0}' is pending finalization. Generating asymmetric key pair...", order.OrderUrl);

                IAsymmetricKeyPair keyPair = RsaAsymmetricKeyPair.Generate(2048);
                Pkcs10CertificationRequest certificationRequest = keyPair.CreateCertificationRequest(domainNames);

                // Wait for finalization...
                order = await acme.FinalizeOrderAsync(order.Payload.Finalize, certificationRequest.GetDerEncoded());
                logger.LogInformation("Certificate order '{0}' has finalized.", order.OrderUrl);

                using MemoryStream certificateStream = new MemoryStream(await acme.GetOrderCertificateAsync(order));
                using StreamReader certificateReader = new StreamReader(certificateStream);

                return X509Certificate2.CreateFromPem(certificateReader.ReadToEnd());
            }

            throw new NotImplementedException();
        }

        private async Task<OrderDetails> AuthorizeOrderAsync(OrderDetails order)
        {
            if (order.Payload.Status == "pending")
            {
                foreach (string authorizationUrl in order.Payload.Authorizations)
                {
                    Authorization authorization = await acme.GetAuthorizationDetailsAsync(authorizationUrl);

                    if (authorization.Status == "pending")
                    {
                        foreach (Challenge challenge in authorization.Challenges)
                        {
                            if (string.IsNullOrEmpty(options.ChallengeType) || options.ChallengeType == challenge.Type)
                            {
                                IChallengeValidationDetails challengeValidation = AuthorizationDecoder.DecodeChallengeValidation(
                                        authorization, challenge.Type, acme.Signer);
                                await challengeValidationRepository.AddAsync(challengeValidation);

                                Challenge answeredChallenge = await acme.AnswerChallengeAsync(challenge.Url);
                                if (answeredChallenge.Error != null)
                                {
                                    logger.LogError("Submitting Challenge Answer reported an error:");
                                    logger.LogError(JsonConvert.SerializeObject(answeredChallenge.Error));
                                }

                                authorization = await acme.GetAuthorizationDetailsAsync(authorizationUrl);
                                if (authorization.Status != "pending")
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return await acme.GetOrderDetailsAsync(order.OrderUrl, order);
        }
    }
}

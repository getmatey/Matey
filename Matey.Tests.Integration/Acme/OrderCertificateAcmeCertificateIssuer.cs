using ACMESharp.Protocol;
using Matey.Acme.Http01;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Matey.Acme.Tests.Integration
{
    public class OrderCertificateAcmeCertificateIssuer
    {
        [Fact]
        public async Task OrderCertificateAsync_Completes_Http01_Acme_Challenge()
        {
            CancellationTokenSource cancellationSource = new CancellationTokenSource();
            IOptions<AcmeOptions> options = Options.Create(new AcmeOptions
            {
                AcceptTermsOfService = true,
                AccountContactEmails = new[] { "lewis.hazell@protonmail.com" },
                ChallengeType = "http-01",
                Environment = "Production",
                Environments = new[]
                    {
                        new AcmeEnvironmentOptions { Name = "Production", CertificateAuthorityUri = "https://localhost/acme/acme/" }
                    },
                Http01 = new AcmeHttp01Options { Bindings = new[] { new AcmeHttp01Binding { HostName = "*", Port = 80 } } },
                KeyExchangeAlgorithm = "RSA"
            });
            ILoggerFactory loggerFactory = new LoggerFactory();
            Http01ChallengeValidationDetailsRepository challengeRepository = new(loggerFactory.CreateLogger<Http01ChallengeValidationDetailsRepository>());
            AcmeHttp01ChallengeResponderHostedService challengeResponder = new(
                options,
                loggerFactory.CreateLogger<AcmeHttp01ChallengeResponderHostedService>(),
                challengeRepository);
            AcmeCertificateIssuer certificateIssuer = new(
                loggerFactory.CreateLogger<AcmeCertificateIssuer>(),
                options,
                new AcmeProtocolClient(new Uri(options.Value.Environments.First().CertificateAuthorityUri), usePostAsGet: true),
                challengeRepository);

            await challengeResponder.StartAsync(cancellationSource.Token);
            await certificateIssuer.OrderCertificateAsync("localhost");
        }
    }
}
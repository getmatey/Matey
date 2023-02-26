using ACMESharp.Authorizations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Matey.Acme.Http
{
    public class AcmeHttpChallengeResponderHostedService : IHostedService, IDisposable
    {
        private readonly HttpListener httpListener = new HttpListener();
        private readonly AcmeHttpOptions options;
        private readonly ILogger<AcmeHttpChallengeResponderHostedService> logger;
        private readonly IChallengeValidationDetailsRepository repository;

        public AcmeHttpChallengeResponderHostedService(
            IOptions<AcmeOptions> options,
            ILogger<AcmeHttpChallengeResponderHostedService> logger,
            IChallengeValidationDetailsRepository repository)
        {
            this.options = options.Value?.Http ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (options.Bindings == null)
            {
                httpListener.Prefixes.Add($"http://{AcmeHttpOptionsDefaults.HostName}:{AcmeHttpOptionsDefaults.Port}/");
            }
            else
            {
                foreach (AcmeHttpBinding binding in options.Bindings)
                {
                    httpListener.Prefixes.Add($"http://{binding.HostName ?? AcmeHttpOptionsDefaults.HostName}:{binding.Port}/");
                }
            }

            httpListener.Start();
            while(httpListener.IsListening)
            {
                HttpListenerContext ctx = await Task.Factory.FromAsync(
                    httpListener.BeginGetContext,
                    httpListener.EndGetContext,
                    httpListener);

                string response = "";
                try
                {
                    response = await HandleRequestAsync(ctx);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                    ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = "500 - Internal Server Error";
                }
                finally
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(response);
                    ctx.Response.ContentLength64 = buffer.Length;
                    await ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    ctx.Response.Close();
                }
            }
        }

        private async Task<string> HandleRequestAsync(HttpListenerContext ctx)
        {
            string fullPath = ctx.Request.Url.ToString().Trim('/');

            try
            {
                IChallengeValidationDetails details = await repository.FindAsync(fullPath);

                if (details is Http01ChallengeValidationDetails httpDetails)
                {
                    logger.LogInformation("Challenge '{0}' found, responding with '{1}'.", fullPath, httpDetails.HttpResourceValue);
                    ctx.Response.ContentType = httpDetails.HttpResourceContentType;
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                    return httpDetails.HttpResourceValue;
                }
                else
                {
                    throw new AcmeChallengeDetailsNotFoundException($"Challenge '{fullPath}' is not a HTTP-01 challenge.");
                }
            }
            catch (AcmeChallengeDetailsNotFoundException ex)
            {
                logger.LogError(ex.Message);
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return "No matching ACME response path";
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            httpListener.Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            httpListener.Close();
        }
    }
}

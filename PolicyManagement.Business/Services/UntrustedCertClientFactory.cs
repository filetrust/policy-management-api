using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Flurl.Http.Configuration;

namespace Glasswall.PolicyManagement.Business.Services
{
    [ExcludeFromCodeCoverage]
    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
        }
    }
}
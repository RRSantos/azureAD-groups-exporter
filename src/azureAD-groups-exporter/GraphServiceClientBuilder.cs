using System;
using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace azureAD_groups_exporter
{
    static class GraphServiceClientBuilder
    {
        private const string BASE_URL = "https://login.microsoftonline.com";
        private const string SCOPE = "https://graph.microsoft.com/.default";

        public static GraphServiceClient Create(string tenantID, string clientId, string clientSecret)
        {
            string instanceUrl = $"{BASE_URL}/{tenantID}";
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(instanceUrl))
                    .Build();

            string[] scopes = new string[] { SCOPE };
            AuthenticationResult result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                return Task.CompletedTask;
            }));
            
            return graphServiceClient;
        }
    }
}

using Azure.Identity;
using Microsoft.Graph;

class Program
{
    public static readonly string tenantId = "54aee102-18b0-4af1-86f3-4a3c3210c6ca";
    public static readonly string clientId = "e78b5831-0c1d-4925-b13e-c399b7a4749c";
    static void Main(string[] args)
    {
        var users = GetUsersAD()?.Result;
        //var b = Test2()?.Result;
        //var c = Test3()?.Result;
    }


    public static async Task<IGraphServiceUsersCollectionPage> GetUsersAD()
    {
        try
        {
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.

            var scopes = new[] { "https://graph.microsoft.com/.default" };


            // Values from app registration
            var clientSecret = "Wsy8Q~d43qjatLBu.eYVQMdZlK50DxH54oAmFb6Y";

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
            var users = await graphClient.Users.Request().GetAsync();
            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async static Task<GraphResponse<GraphServiceUsersCollectionResponse>> Test2()
    {
        try
        {
            var scopes = new[] { "User.Read" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // Callback function that receives the user prompt
            // Prompt contains the generated device code that use must
            // enter during the auth process in the browser
            Func<DeviceCodeInfo, CancellationToken, Task> callback = (code, cancellation) =>
            {
                Console.WriteLine(code.Message);
                return Task.FromResult(0);
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
            var deviceCodeCredential = new DeviceCodeCredential(
                callback, tenantId, clientId, options);

            var graphClient = new GraphServiceClient(deviceCodeCredential, scopes);
            return await graphClient.Users.Request().GetResponseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async static Task<GraphResponse<GraphServiceUsersCollectionResponse>> Test3()
    {
        try
        {
            var scopes = new[] { "User.Read" };
            
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var userName = "hiberus@grupolobe.com";
            var password = "iritec1O";

            var userNamePasswordCredential = new UsernamePasswordCredential(
                userName, password, tenantId, clientId, options);

            var graphClient = new GraphServiceClient(userNamePasswordCredential, scopes);
            return await graphClient.Users.Request().GetResponseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }


}




using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PacoMediaTranscriber.Data;
using PacoMediaTranscriber.Services;

namespace PacoMediaTranscriber
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Base HttpClient (same-origin)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            // AWS API HttpClient (this is the one you're registering for AWS work)
            builder.Services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://your-api-id.execute-api.us-east-1.amazonaws.com/prod/");
            });

            // For accessing Microsoft Graph

            builder.Services.AddGraphClient(builder.Configuration);

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
            });


            await builder.Build().RunAsync();
        }
    }
}

using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PacoMediaTranscriber.Data
{
    public static class GraphClientExtensions
    {
        /// <summary>
        /// Registers an HttpClient named "GraphAPI" that automatically
        /// attaches an access token for Microsoft Graph.
        /// </summary>
        public static IServiceCollection AddGraphClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var baseUrl = configuration["MicrosoftGraph:BaseUrl"]?.TrimEnd('/')
                          ?? "https://graph.microsoft.com/v1.0";

            var scopesConfig = configuration["MicrosoftGraph:Scopes"] ?? "user.read";
            var scopes = scopesConfig.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // HttpClient for Microsoft Graph
            services.AddHttpClient("GraphAPI", client =>
            {
                client.BaseAddress = new Uri(baseUrl + "/");
            })
            .AddHttpMessageHandler(sp =>
            {
                var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { baseUrl },
                        scopes: scopes);

                return handler;
            });

            return services;
        }
    }
}

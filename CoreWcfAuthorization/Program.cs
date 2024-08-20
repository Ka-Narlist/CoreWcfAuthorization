using CoreWCF.Channels;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Web.Services.Description;
using CoreWcfAuthorization.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace CoreWcfAuthorization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Policy1", policy =>
                {
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid", "xyz");
                });

                //options.AddPolicy("Policy1", new AuthorizationPolicyBuilder(NegotiateDefaults.AuthenticationScheme)
                //                                .RequireAuthenticatedUser()
                //                                .RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid", "xyz")
                //                                .Build());
            });

            builder.Services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Trace);

            builder.Services.AddServiceModelServices().AddServiceModelMetadata();
            builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            var app = builder.Build();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    Thread.CurrentPrincipal = context.User;
                }

                // Log claims for debugging
                if (context.User.Identity.IsAuthenticated)
                {
                    foreach (var claim in context.User.Claims)
                    {
                        Console.WriteLine($"Middleware - Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Middleware - User is not authenticated");
                }
                await next();
            });

            app.UseServiceModel(builder =>
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.InheritedFromHost;

                builder.AddService<TestService>(serviceOptions =>
                {
                    serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
                })
                .AddServiceEndpoint<TestService, ITestService>(binding, "/TestService/bhttps");

                var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpsGetEnabled = true;
            });

            app.Run();
        }
    }
}

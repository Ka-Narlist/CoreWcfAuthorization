using CoreWcfAuthorization.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoreWcfAuthorization
{
    public class TestService : ITestService
    {
        [Authorize(Policy = "Policy1")]
        public string Test1()
        {
            // Inspect claims within the WCF service method
            var user = ClaimsPrincipal.Current;
            if (user != null)
            {
                foreach (var claim in user.Claims)
                {
                    Console.WriteLine($"Service Method - Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("Service Method - User is not authenticated");
            }

            return "yes";
        }
    }
}

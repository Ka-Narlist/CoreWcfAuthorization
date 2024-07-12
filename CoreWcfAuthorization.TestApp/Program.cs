using CoreWcfAuthorization.Contracts;
using System.ServiceModel;

namespace CoreWcfAuthorization.TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

                var endpoint = new EndpointAddress("https://localhost:7206/TestService/bhttps");
                var channelFactory = new ChannelFactory<ITestService>(binding, endpoint);

                var client = channelFactory.CreateChannel();

                var result = client.Test1();
                Console.WriteLine(result);


                channelFactory.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}

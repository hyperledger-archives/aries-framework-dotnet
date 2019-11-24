using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries;
using Hyperledger.Aries.Routing;

namespace EdgeConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            var host1 = CreateHostBuilder("Edge1").Build();
            var host2 = CreateHostBuilder("Edge2").Build();

            try
            {
                await host1.StartAsync();
                await host2.StartAsync();

                var context1 = await host1.Services.GetRequiredService<IAgentProvider>().GetContextAsync();
                var context2 = await host2.Services.GetRequiredService<IAgentProvider>().GetContextAsync();

                var (invitation, record1) = await host1.Services.GetRequiredService<IConnectionService>().CreateInvitationAsync(context1, new InviteConfiguration { AutoAcceptConnection = true });

                var (request, record2) = await host2.Services.GetRequiredService<IConnectionService>().CreateRequestAsync(context2, invitation);
                await host2.Services.GetRequiredService<IMessageService>().SendAsync(context2.Wallet, request, record2);

                await host1.Services.GetRequiredService<IEdgeClientService>().FetchInboxAsync(context1);
                await host2.Services.GetRequiredService<IEdgeClientService>().FetchInboxAsync(context2);

                record1 = await host1.Services.GetRequiredService<IConnectionService>().GetAsync(context1, record1.Id);
                record2 = await host2.Services.GetRequiredService<IConnectionService>().GetAsync(context2, record2.Id);

                await host1.Services.GetRequiredService<IEdgeClientService>().AddDeviceAsync(context1, new AddDeviceInfoMessage { DeviceId = "123", DeviceVendor = "Apple" });

                Console.WriteLine($"Record1 is {record1.State}, Record2 is {record2.State}");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Connected");
        }

        public static IHostBuilder CreateHostBuilder(string walletId) =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAriesFramework(builder =>
                    {
                        builder.RegisterEdgeAgent(options =>
                        {
                            options.EndpointUri = "http://localhost:5000";
                            options.WalletConfiguration.Id = walletId;
                        });
                    });
                });
    }
}

using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Indy.WalletApi;
using Moq;

namespace Hyperledger.TestHarness.Utils
{
    public static class ServiceUtils
    {
        public static IProvisioningService GetDefaultMockProvisioningService(string endpointUri = TestConstants.DefaultMockUri, string masterSecret = TestConstants.DefaultMasterSecret, string verkey = TestConstants.DefaultVerkey)
        {
            var provisionRecord = new Mock<ProvisioningRecord>();

            provisionRecord.SetupGet(x => x.Owner).Returns(new AgentOwner
            {
                ImageUrl = "http://default.com",
                Name = "Tester"
            });

            provisionRecord.SetupGet(x => x.Endpoint).Returns(new AgentEndpoint(endpointUri, "", new[] { verkey }));
            provisionRecord.SetupGet(x => x.MasterSecretId).Returns(masterSecret);

            var provisionServiceMock = new Mock<IProvisioningService>();

            provisionServiceMock.Setup(x => x.GetProvisioningAsync(It.IsAny<Wallet>()))
                .Returns(Task.FromResult(provisionRecord.Object));

            return provisionServiceMock.Object;
        }
    }
}

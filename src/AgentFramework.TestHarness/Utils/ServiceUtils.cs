using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.WalletApi;
using Moq;

namespace AgentFramework.TestHarness.Utils
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

            provisionRecord.SetupGet(x => x.Endpoint).Returns(new AgentEndpoint(endpointUri, "", verkey));
            provisionRecord.SetupGet(x => x.MasterSecretId).Returns(masterSecret);

            var provisionServiceMock = new Mock<IProvisioningService>();

            provisionServiceMock.Setup(x => x.GetProvisioningAsync(It.IsAny<Wallet>()))
                .Returns(Task.FromResult(provisionRecord.Object));

            return provisionServiceMock.Object;
        }
    }
}

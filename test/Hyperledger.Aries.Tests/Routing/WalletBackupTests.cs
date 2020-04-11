using Hyperledger.Indy.DidApi;
using Hyperledger.TestHarness;
using System.Threading.Tasks;
using Hyperledger.Aries.Extensions;
using Xunit;
using Hyperledger.Indy;

namespace Hyperledger.Aries.Tests.Routing
{
    public class WalletBackupTests : TestSingleWallet
    {
        [Fact]
        public async Task TestDidRotateKeys()
        {
            const string backupDid = "22222222AriesBackupDid";
            const string seed = "00000000000000000000000000000001";
            const string seed1 = "00000000000000000000000000000002";

            var did = await Did.CreateAndStoreMyDidAsync(Context.Wallet, new
            {
                did = backupDid,
                seed = seed
            }.ToJson());
            Assert.Equal(did.Did, backupDid);

            var ex = await Assert.ThrowsAsync<IndyException>(async () => await Did.CreateAndStoreMyDidAsync(Context.Wallet, new
            {
                did = backupDid,
                seed = seed1
            }.ToJson()));
            Assert.Equal(600, ex.SdkErrorCode);

            var result = await Did.ReplaceKeysStartAsync(Context.Wallet, backupDid, new
            {
                did = backupDid,
                seed = seed1
            }.ToJson());
            await Did.ReplaceKeysApplyAsync(Context.Wallet, backupDid);

            var newKey = await Did.KeyForLocalDidAsync(Context.Wallet, backupDid);

            Assert.NotEqual(newKey, did.VerKey);
            Assert.Equal(newKey, result);
        }
    }
}

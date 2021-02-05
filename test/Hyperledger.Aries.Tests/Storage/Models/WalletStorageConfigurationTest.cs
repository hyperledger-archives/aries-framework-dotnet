using FluentAssertions;
using Hyperledger.Aries.Storage;
using Xunit;

namespace Hyperledger.Aries.Tests.Storage.Models
{
    public class WalletStorageConfigurationTest
    {
        private const string Path = "Path";
        private const string Url = "Url";
        private const string WalletScheme = "WalletScheme";
        
        [Fact(DisplayName = "Create wallet configuration model should correctly set and stringify all parameters")]
        public void CreateInboxRecordAsync()
        {
            WalletConfiguration.WalletStorageConfiguration walletStorageConfiguration = new WalletConfiguration.WalletStorageConfiguration()
            {
                Path = Path,
                Url = Url,
                WalletScheme = WalletScheme
            };

            walletStorageConfiguration.Path.Should().Be(Path);
            walletStorageConfiguration.Url.Should().Be(Url);
            walletStorageConfiguration.WalletScheme.Should().Be(WalletScheme);
            walletStorageConfiguration.ToString().Should().Be(
                "WalletStorageConfiguration: " +
                $"Path={Path}" + 
                $"Url={Url}" +
                $"WalletScheme={WalletScheme}");
        }
    }
}

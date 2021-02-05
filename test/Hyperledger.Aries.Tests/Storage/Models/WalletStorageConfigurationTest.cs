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
        private const string DatabaseName = "DatabaseName";
        private const string Tls = "on";
        private const int MaxConnections = 10;
        private const int MinIdleCount = 10;
        private const int ConnectionTimeout = 10;

        [Fact(DisplayName = "Wallet configuration model should correctly set and stringify all parameters")]
        public void WalletStorageConfigurationModel()
        {
            WalletConfiguration.WalletStorageConfiguration walletStorageConfiguration = new WalletConfiguration.WalletStorageConfiguration
            {
                Path = Path,
                Url = Url,
                WalletScheme = WalletScheme,
                DatabaseName = DatabaseName,
                Tls = Tls,
                MaxConnections = MaxConnections,
                MinIdleCount = MinIdleCount,
                ConnectionTimeout = ConnectionTimeout
            };

            walletStorageConfiguration.Should().BeEquivalentTo(new
            {
                Path,
                Url,
                WalletScheme,
                DatabaseName,
                Tls,
                MaxConnections,
                MinIdleCount,
                ConnectionTimeout
            });
            walletStorageConfiguration.ToString().Should().Be(
                "WalletStorageConfiguration: " +
                $"Path={Path}" +
                $"Url={Url}" +
                $"Tls={Tls}" +
                $"WalletScheme={WalletScheme}" +
                $"DatabaseName={DatabaseName}" +
                $"MaxConnections={MaxConnections}" +
                $"MinIdleCount={MinIdleCount}" +
                $"ConnectionTimeout={ConnectionTimeout}");
        }

        [Fact(DisplayName = "Wallet configuration model should use default values if they are not specified")]
        public void WalletStorageConfigurationModelDefaults()
        {
            WalletConfiguration.WalletStorageConfiguration walletStorageConfiguration = new WalletConfiguration.WalletStorageConfiguration();

            walletStorageConfiguration.Should().BeEquivalentTo(new
            {
                MaxConnections = 5,
                MinIdleCount = 0,
                ConnectionTimeout = 5
            });
        }
    }
}

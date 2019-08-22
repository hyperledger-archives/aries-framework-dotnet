using System;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Runtime;
using Hyperledger.Indy.WalletApi;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class SearchTests : IAsyncLifetime
    {
        private const string Config = "{\"id\":\"search_test_wallet\"}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private Wallet _wallet;

        private readonly IWalletRecordService _recordService;

        public SearchTests()
        {
            _recordService = new DefaultWalletRecordService();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Wallet.CreateWalletAsync(Config, Credentials);
            }
            catch (WalletExistsException)
            {
            }
            finally
            {
                _wallet = await Wallet.OpenWalletAsync(Config, Credentials);
            }
        }

        [Fact]
        public async Task CanFilterSearchableProperties()
        {
            await _recordService.AddAsync(_wallet,
                new ConnectionRecord {Id= "1", State = ConnectionState.Invited});
            await _recordService.AddAsync(_wallet,
                new ConnectionRecord {Id = "2", State = ConnectionState.Connected});

            var searchResult = await _recordService.SearchAsync<ConnectionRecord>(_wallet,
                SearchQuery.Equal(nameof(ConnectionRecord.State), ConnectionState.Invited.ToString("G")), null, 10);

            Assert.Single(searchResult);
            Assert.Equal("1", searchResult.Single().Id);
        }

        [Fact]
        public async Task CanSearchMulipleProperties()
        {
            var record1 = new ConnectionRecord {State = ConnectionState.Connected, Id = "1"};
            var record2 = new ConnectionRecord
            {
                State = ConnectionState.Connected,
                Id = "2"
            };
            record2.SetTag("tagName", "tagValue");

            var record3 = new ConnectionRecord
            {
                State = ConnectionState.Invited,
                Id = "3"
            };
            record3.SetTag("tagName", "tagValue");
            

            await _recordService.AddAsync(_wallet, record1);
            await _recordService.AddAsync(_wallet, record2);
            await _recordService.AddAsync(_wallet, record3);


            var searchResult = await _recordService.SearchAsync<ConnectionRecord>(_wallet,
                SearchQuery.And(
                    SearchQuery.Equal("State", ConnectionState.Connected.ToString("G")),
                    SearchQuery.Equal("tagName", "tagValue")

                ), null, 10);

            Assert.Single(searchResult);
            Assert.Equal("2", searchResult.Single().Id);
        }

        [Fact]
        public async Task ReturnsEmptyIfNoRecordsMatchCriteria()
        {
            var record = new ConnectionRecord
            {
                Id = Guid.NewGuid().ToString(),
                State = ConnectionState.Invited
            };
            record.SetTag("tagName", "tagValue");

            await _recordService.AddAsync(_wallet, record);
            await _recordService.AddAsync(_wallet,
                new ConnectionRecord {Id = Guid.NewGuid().ToString(), State = ConnectionState.Connected});

            var searchResult = await _recordService.SearchAsync<ConnectionRecord>(_wallet,
                SearchQuery.And(
                    SearchQuery.Equal("State", ConnectionState.Connected.ToString("G")),
                    SearchQuery.Equal("tagName", "tagValue")
                ), null, 10);

            Assert.Empty(searchResult);
        }

        public async Task DisposeAsync()
        {
            await _wallet.CloseAsync();
            await Wallet.DeleteWalletAsync(Config, Credentials);
        }
    }
}
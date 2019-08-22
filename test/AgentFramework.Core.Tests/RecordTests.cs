using System;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.TestHarness;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class RecordTests : TestSingleWallet
    {
        [Fact]
        public async Task CanStoreAndRetrieveRecordWithTags()
        {
            var record = new ConnectionRecord { Id = "123" };
            record.SetTag("tag1", "tagValue1");

            await recordService.AddAsync(Context.Wallet, record);

            var retrieved = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, "123");

            Assert.NotNull(retrieved);
            Assert.Equal(retrieved.Id, record.Id);
            Assert.NotNull(retrieved.GetTag("tag1"));
            Assert.Equal("tagValue1", retrieved.GetTag("tag1"));
        }

        [Fact]
        public async Task CanStoreAndRetrieveRecordWithTagsUsingSearch()
        {
            var tagName = Guid.NewGuid().ToString();
            var tagValue = Guid.NewGuid().ToString();

            var record = new ConnectionRecord { Id = Guid.NewGuid().ToString() };
            record.SetTag(tagName, tagValue);

            await recordService.AddAsync(Context.Wallet, record);

            var search =
                await recordService.SearchAsync<ConnectionRecord>(Context.Wallet,
                    SearchQuery.Equal(tagName, tagValue), null, 100);

            var retrieved = search.Single();

            Assert.NotNull(retrieved);
            Assert.Equal(retrieved.Id, record.Id);
            Assert.NotNull(retrieved.GetTag(tagName));
            Assert.Equal(tagValue, retrieved.GetTag(tagName));
        }

        [Fact]
        public async Task CanUpdateRecordWithTags()
        {
            var tagName = Guid.NewGuid().ToString();
            var tagValue = Guid.NewGuid().ToString();

            var id = Guid.NewGuid().ToString();

            var record = new ConnectionRecord { Id = id };
            record.SetTag(tagName, tagValue);

            await recordService.AddAsync(Context.Wallet, record);

            var retrieved = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, id);

            retrieved.MyDid = "123";
            retrieved.SetTag(tagName, "value");

            await recordService.UpdateAsync(Context.Wallet, retrieved);

            var updated = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, id);

            Assert.NotNull(updated);
            Assert.Equal(updated.Id, record.Id);
            Assert.NotNull(updated.GetTag(tagName));
            Assert.Equal("value", updated.GetTag(tagName));
            Assert.Equal("123", updated.MyDid);
        }

        [Fact]
        public async Task ReturnsNullForNonExistentRecord()
        {
            var record = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, Guid.NewGuid().ToString());
            Assert.Null(record);
        }

        [Fact]
        public async Task ReturnsEmptyListForNonExistentRecord()
        {
            var record = await recordService.SearchAsync<ConnectionRecord>(
                Context.Wallet,
                SearchQuery.Equal(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), null, 100);
            Assert.False(record.Any());
        }

        [Fact]
        public void InitialConnectionRecordIsInvitedAndHasTag()
        {
            var record = new ConnectionRecord();

            Assert.True(record.State == ConnectionState.Invited);
            Assert.True(record.GetTag(nameof(ConnectionRecord.State)) == ConnectionState.Invited.ToString("G"));
        }

        [Fact]
        public void InitialCredentialRecordIsOfferedAndHasTag()
        {
            var record = new CredentialRecord();

            Assert.True(record.State == CredentialState.Offered);
            Assert.True(record.GetTag(nameof(CredentialRecord.State)) == CredentialState.Offered.ToString("G"));
        }

        [Fact]
        public async Task CreatedAtPopulatedOnStoredRecord()
        {
            var record = new ConnectionRecord { Id = "123" };

            Assert.Null(record.CreatedAtUtc);

            await recordService.AddAsync(Context.Wallet, record);

            var retrieved = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, "123");

            Assert.NotNull(retrieved);
            Assert.Equal(retrieved.Id, record.Id);
            Assert.NotNull(retrieved.CreatedAtUtc);
        }

        [Fact]
        public async Task UpdateAtPopulatedOnUpdatedRecord()
        {
            var record = new ConnectionRecord { Id = "123" };

            await recordService.AddAsync(Context.Wallet, record);

            var retrieved = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, "123");

            Assert.NotNull(retrieved);
            Assert.Equal(retrieved.Id, record.Id);
            Assert.Null(retrieved.UpdatedAtUtc);

            await recordService.UpdateAsync(Context.Wallet, retrieved);

            retrieved = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, "123");

            Assert.NotNull(retrieved);
            Assert.Equal(retrieved.Id, record.Id);
            Assert.NotNull(retrieved.UpdatedAtUtc);
        }

        [Fact]
        public async Task ReturnsRecordsFilteredByCreatedAt()
        {
            var record = new ConnectionRecord { Id = "123" };
            await recordService.AddAsync(Context.Wallet, record);

            await Task.Delay(TimeSpan.FromSeconds(1));
            var now = DateTime.UtcNow;
            await Task.Delay(TimeSpan.FromSeconds(1));

            record = new ConnectionRecord {Id = "456"};
            await recordService.AddAsync(Context.Wallet, record);

            var records = await recordService.SearchAsync<ConnectionRecord>(
                Context.Wallet,
                SearchQuery.Greater(nameof(ConnectionRecord.CreatedAtUtc), now), null, 100);

            Assert.True(records.Count == 1);
            Assert.True(records[0].Id == "456");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using Hyperledger.Indy.NonSecretsApi;
using Hyperledger.Indy.WalletApi;
using Newtonsoft.Json;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultWalletRecordService : IWalletRecordService
    {
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>Initializes a new instance of the <see cref="DefaultWalletRecordService"/> class.</summary>
        public DefaultWalletRecordService()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <inheritdoc />
        public virtual Task AddAsync<T>(Wallet wallet, T record)
            where T : RecordBase, new()
        {
            record.CreatedAtUtc = DateTime.UtcNow;

            return NonSecrets.AddRecordAsync(wallet,
                record.TypeName,
                record.Id,
                record.ToJson(_jsonSettings),
                record.Tags.ToJson());
        }

        /// <inheritdoc />
        public virtual async Task<List<T>> SearchAsync<T>(Wallet wallet, ISearchQuery query, SearchOptions options, int count)
            where T : RecordBase, new()
        {
            using (var search = await NonSecrets.OpenSearchAsync(wallet, new T().TypeName,
                (query ?? SearchQuery.Empty).ToJson(),
                (options ?? new SearchOptions()).ToJson()))
            {
                var result = JsonConvert.DeserializeObject<SearchResult>(await search.NextAsync(wallet, count), _jsonSettings);
                // TODO: Add support for pagination

                return result.Records?
                           .Select(x =>
                           {
                               var record = JsonConvert.DeserializeObject<T>(x.Value);
                               foreach (var tag in x.Tags)
                                   record.Tags[tag.Key] = tag.Value;
                               return record;
                           })
                           .ToList()
                       ?? new List<T>();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(Wallet wallet, RecordBase record)
        {
            record.UpdatedAtUtc = DateTime.UtcNow;

            await NonSecrets.UpdateRecordValueAsync(wallet,
                record.TypeName,
                record.Id,
                record.ToJson(_jsonSettings));

            await NonSecrets.UpdateRecordTagsAsync(wallet,
                record.TypeName,
                record.Id,
                record.Tags.ToJson(_jsonSettings));
        }

        /// <inheritdoc />
        public virtual async Task<T> GetAsync<T>(Wallet wallet, string id) where T : RecordBase, new()
        {
            try
            {
                var recordJson = await NonSecrets.GetRecordAsync(wallet,
                    new T().TypeName,
                    id,
                    new SearchOptions().ToJson());

                if (recordJson == null) return null;

                var item = JsonConvert.DeserializeObject<SearchItem>(recordJson, _jsonSettings);

                var record = JsonConvert.DeserializeObject<T>(item.Value, _jsonSettings);

                foreach (var tag in item.Tags)
                    record.Tags[tag.Key] = tag.Value;

                return record;
            }
            catch (WalletItemNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync<T>(Wallet wallet, string id) where T : RecordBase, new()
        {
            try
            {
                await NonSecrets.DeleteRecordAsync(wallet,
                     new T().TypeName,
                     id);

                return true;
            }
            catch (WalletItemNotFoundException)
            {
                return false;
            }
        }
    }
}
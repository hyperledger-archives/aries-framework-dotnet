using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Indy.NonSecretsApi;
using Hyperledger.Indy.WalletApi;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Storage
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
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new AgentEndpointJsonConverter(),
                    new AttributeFilterConverter()
                }
            };
        }

        /// <inheritdoc />
        public virtual Task AddAsync<T>(Wallet wallet, T record)
            where T : RecordBase, new()
        {
            Debug.WriteLine($"Adding record of type {record.TypeName} with Id {record.Id}");

            record.CreatedAtUtc = DateTime.UtcNow;

            return NonSecrets.AddRecordAsync(wallet,
                record.TypeName,
                record.Id,
                record.ToJson(_jsonSettings),
                record.Tags.ToJson());
        }

        /// <inheritdoc />
        public virtual async Task<List<T>> SearchAsync<T>(Wallet wallet, ISearchQuery query, SearchOptions options, int count, int skip)
            where T : RecordBase, new()
        {
            using (var search = await NonSecrets.OpenSearchAsync(wallet, new T().TypeName,
                (query ?? SearchQuery.Empty).ToJson(),
                (options ?? new SearchOptions()).ToJson()))
            {
                if(skip > 0) {
                    await search.NextAsync(wallet, skip);
                }
                var result = JsonConvert.DeserializeObject<SearchResult>(await search.NextAsync(wallet, count), _jsonSettings);

                return result.Records?
                           .Select(x =>
                           {
                               var record = JsonConvert.DeserializeObject<T>(x.Value, _jsonSettings);
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
                var record = await GetAsync<T>(wallet, id);
                var typeName = new T().TypeName;

                await NonSecrets.DeleteRecordTagsAsync(
                    wallet: wallet,
                    type: typeName,
                    id: id,
                    tagsJson: record.Tags.Select(x => x.Key).ToArray().ToJson());
                await NonSecrets.DeleteRecordAsync(
                    wallet: wallet,
                    type: typeName,
                    id: id);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Couldn't delete record: {e}");
                return false;
            }
        }
    }
}
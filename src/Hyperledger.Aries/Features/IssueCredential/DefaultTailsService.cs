using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.BlobStorageApi;
using Hyperledger.Indy.PoolApi;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <inheritdoc />
    public class DefaultTailsService : ITailsService
    {
        /// <summary>The BLOB readers</summary>
        protected static readonly ConcurrentDictionary<string, BlobStorageReader> BlobReaders =
            new ConcurrentDictionary<string, BlobStorageReader>();

        /// <summary>The ledger service</summary>
        // ReSharper disable InconsistentNaming
        protected readonly ILedgerService LedgerService;

        /// <summary>The HTTP client</summary>
        protected readonly HttpClient HttpClient;
        // ReSharper restore InconsistentNaming

        /// <summary>Initializes a new instance of the <see cref="DefaultTailsService"/> class.</summary>
        /// <param name="ledgerService">The ledger service.</param>
        /// <param name="httpClientFactory"></param>
        public DefaultTailsService(
            ILedgerService ledgerService,
            IHttpClientFactory httpClientFactory)
        {
            LedgerService = ledgerService;
            HttpClient = httpClientFactory.CreateClient();
        }

        /// <inheritdoc />
        public virtual async Task<BlobStorageReader> OpenTailsAsync(string filename)
        {
            var baseDir = EnvironmentUtils.GetTailsPath();

            var tailsWriterConfig = new
            {
                base_dir = baseDir,
                uri_pattern = string.Empty,
                file = filename
            };

            if (BlobReaders.TryGetValue(filename, out var blobReader))
            {
                return blobReader;
            }

            blobReader = await BlobStorage.OpenReaderAsync("default", tailsWriterConfig.ToJson());
            BlobReaders.TryAdd(filename, blobReader);
            return blobReader;
        }

        /// <inheritdoc />
        public virtual async Task<BlobStorageWriter> CreateTailsAsync()
        {
            var tailsWriterConfig = new
            {
                base_dir = EnvironmentUtils.GetTailsPath(),
                uri_pattern = string.Empty
            };

            var blobWriter = await BlobStorage.OpenWriterAsync("default", tailsWriterConfig.ToJson());
            return blobWriter;
        }

        /// <inheritdoc />
        public virtual async Task<string> EnsureTailsExistsAsync(Pool pool, string revocationRegistryId)
        {
            var revocationRegistry =
                await LedgerService.LookupRevocationRegistryDefinitionAsync(pool, revocationRegistryId);
            var tailsUri = JObject.Parse(revocationRegistry.ObjectJson)["value"]["tailsLocation"].ToObject<string>();
            var tailsFileName = JObject.Parse(revocationRegistry.ObjectJson)["value"]["tailsHash"].ToObject<string>();

            var tailsfile = Path.Combine(EnvironmentUtils.GetTailsPath(), tailsFileName);

            if (!Directory.Exists(EnvironmentUtils.GetTailsPath()))
            {
                Directory.CreateDirectory(EnvironmentUtils.GetTailsPath());
            }

            if (!File.Exists(tailsfile))
            {
                try
                {
                    File.WriteAllBytes(
                        path: tailsfile,
                        bytes: await HttpClient.GetByteArrayAsync(new Uri(tailsUri)));
                }
                catch (Exception e)
                {
                    throw new AriesFrameworkException(
                        errorCode: ErrorCode.RevocationRegistryUnavailable,
                        message: $"Unable to retrieve revocation registry from the specified URL '{tailsUri}'. Error: {e.Message}");
                }
            }

            return Path.GetFileName(tailsfile);
        }
    }
}
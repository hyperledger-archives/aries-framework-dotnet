using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.BlobStorageApi;
using Hyperledger.Indy.PoolApi;
using Microsoft.Extensions.Options;
using Multiformats.Base;
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
        /// <summary>
        /// The agent options
        /// </summary>
        protected readonly AgentOptions AgentOptions;

        /// <summary>The HTTP client</summary>
        protected readonly HttpClient HttpClient;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTailsService" /> class.
        /// </summary>
        /// <param name="ledgerService">The ledger service.</param>
        /// <param name="agentOptions">The agent options.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public DefaultTailsService(
            ILedgerService ledgerService,
            IOptions<AgentOptions> agentOptions,
            IHttpClientFactory httpClientFactory)
        {
            LedgerService = ledgerService;
            AgentOptions = agentOptions.Value;
            HttpClient = httpClientFactory.CreateClient();
        }

        /// <inheritdoc />
        public virtual async Task<BlobStorageReader> OpenTailsAsync(string filename)
        {
            var baseDir = AgentOptions.RevocationRegistryDirectory;

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
                base_dir = AgentOptions.RevocationRegistryDirectory,
                uri_pattern = string.Empty
            };

            var blobWriter = await BlobStorage.OpenWriterAsync("default", tailsWriterConfig.ToJson());
            return blobWriter;
        }

        /// <inheritdoc />
        public virtual async Task<string> EnsureTailsExistsAsync(IAgentContext agentContext, string revocationRegistryId)
        {
            var revocationRegistry = await LedgerService.LookupRevocationRegistryDefinitionAsync(agentContext, revocationRegistryId);
            var tailsUri = JObject.Parse(revocationRegistry.ObjectJson)["value"]["tailsLocation"].ToObject<string>();
            var tailsFileName = JObject.Parse(revocationRegistry.ObjectJson)["value"]["tailsHash"].ToObject<string>();

            var tailsfile = Path.Combine(AgentOptions.RevocationRegistryDirectory, tailsFileName);
            var hash = Multibase.Base58.Decode(tailsFileName);

            if (!Directory.Exists(AgentOptions.RevocationRegistryDirectory))
            {
                Directory.CreateDirectory(AgentOptions.RevocationRegistryDirectory);
            }

            try
            {
                if (!File.Exists(tailsfile))
                {
                    var bytes = await HttpClient.GetByteArrayAsync(new Uri(tailsUri));

                    // Check hash
                    using var sha256 = SHA256.Create();
                    var computedHash = sha256.ComputeHash(bytes);

                    if (!computedHash.SequenceEqual(hash))
                    {
                        throw new Exception("Tails file hash didn't match");
                    }

                    File.WriteAllBytes(
                        path: tailsfile,
                        bytes: bytes);
                }
            }
            catch (Exception e)
            {
                throw new AriesFrameworkException(
                    errorCode: ErrorCode.RevocationRegistryUnavailable,
                    message: $"Unable to retrieve revocation registry from the specified URL '{tailsUri}'. Error: {e.Message}");
            }

            return Path.GetFileName(tailsfile);
        }
    }
}
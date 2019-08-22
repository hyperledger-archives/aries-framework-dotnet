using System.Threading.Tasks;
using Hyperledger.Indy.BlobStorageApi;
using Hyperledger.Indy.PoolApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Tails Service.
    /// </summary>
    public interface ITailsService
    {
        /// <summary>
        /// Opens an existing tails file and returns a handle.
        /// </summary>
        /// <returns>The tails reader async.</returns>
        /// <param name="filename">The tails filename.</param>
        Task<BlobStorageReader> OpenTailsAsync(string filename);

        /// <summary>
        /// Gets the BLOB storage writer async.
        /// </summary>
        /// <returns>The BLOB storage writer async.</returns>
        Task<BlobStorageWriter> CreateTailsAsync();

        /// <summary>
        /// Check if the tails filename exists locally and download latest version if it doesn't.
        /// </summary>
        /// <param name="pool">Pool.</param>
        /// <param name="revocationRegistryId">Revocation registry identifier.</param>
        /// <returns>
        /// The name of the tails file
        /// </returns>
        Task<string> EnsureTailsExistsAsync(Pool pool, string revocationRegistryId);
    }
}
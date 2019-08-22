using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Utils;

namespace AgentFramework.Core.Extensions
{
    /// <summary>
    /// A collection of convenience methods for the <see cref="ICredentialService"/> class.
    /// </summary>
    public static class CredentialServiceExtensions
    {
        /// <summary>Retrieves a list of credential offers</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">Count.</param>
        /// <returns>The offers async.</returns>
        public static Task<List<CredentialRecord>> ListOffersAsync(this ICredentialService credentialService,
            IAgentContext context, int count = 100)
            => credentialService.ListAsync(context,
               SearchQuery.Equal(nameof(CredentialRecord.State), CredentialState.Offered.ToString("G")), count);

        /// <summary>Retrieves a list of credential requests</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">Count.</param>
        /// <returns>The requests async.</returns>
        public static Task<List<CredentialRecord>> ListRequestsAsync(this ICredentialService credentialService,
            IAgentContext context, int count = 100)
            => credentialService.ListAsync(context,
                SearchQuery.Equal(nameof(CredentialRecord.State), CredentialState.Requested.ToString("G")), count);

        /// <summary>Retrieves a list of issued credentials</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">Count.</param>
        /// <returns>The issued credentials async.</returns>
        public static Task<List<CredentialRecord>> ListIssuedCredentialsAsync(this ICredentialService credentialService,
            IAgentContext context, int count = 100)
            => credentialService.ListAsync(context,
                SearchQuery.Equal(nameof(CredentialRecord.State), CredentialState.Issued.ToString("G")), count);

        /// <summary>Retrieves a list of revoked credentials</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">Count.</param>
        /// <returns>The revoked credentials async.</returns>
        public static Task<List<CredentialRecord>> ListRevokedCredentialsAsync(
            this ICredentialService credentialService, IAgentContext context, int count = 100)
            => credentialService.ListAsync(context,
                SearchQuery.Equal(nameof(CredentialRecord.State), CredentialState.Revoked.ToString("G")), count);

        /// <summary>Retrieves a list of rejected/declined credentials.
        /// Rejected credentials will only be found in the issuer wallet, as the rejection is not communicated back to the holder.</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">Count.</param>
        /// <returns>The rejected credentials async.</returns>
        public static Task<List<CredentialRecord>> ListRejectedCredentialsAsync(
            this ICredentialService credentialService, IAgentContext context, int count = 100)
            => credentialService.ListAsync(context,
                SearchQuery.Equal(nameof(CredentialRecord.State), CredentialState.Rejected.ToString("G")), count);

        /// <summary>Retrieves a credential record by its thread id.</summary>
        /// <param name="credentialService">Credential service.</param>
        /// <param name="context">The context.</param>
        /// <param name="threadId">The thread id.</param>
        /// <returns>The credential record.</returns>
        public static async Task<CredentialRecord> GetByThreadIdAsync(
            this ICredentialService credentialService, IAgentContext context, string threadId)
        {
            var search = await credentialService.ListAsync(context, SearchQuery.Equal(TagConstants.LastThreadId, threadId), 100);

            if (search.Count == 0)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, $"Credential record not found by thread id : {threadId}");

            if (search.Count > 1)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState, $"Multiple credential records found by thread id : {threadId}");

            return search.Single();
        }
    }
}
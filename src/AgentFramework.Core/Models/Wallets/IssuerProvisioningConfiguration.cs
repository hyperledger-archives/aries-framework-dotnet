using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.DidApi;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// Issuer provisioning configuraton.
    /// </summary>
    public class IssuerProvisioningConfiguration : ProvisioningConfiguration
    {
        /// <summary>
        /// Gets or sets the issuer seed used to generate deterministic DID and Verkey. (32 characters)
        /// <remarks>Leave <c>null</c> to generate random issuer did and verkey</remarks>
        /// </summary>
        /// <value>
        /// The issuer seed.
        /// </value>
        public string IssuerSeed { get; set; }

        /// <summary>
        /// Gets or sets the tails service base URI.
        /// </summary>
        /// <value>The tails base URI.</value>
        public Uri TailsBaseUri { get; set; }

        /// <summary>
        /// Configures the agent wallet with issuer configuration
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="record">Record.</param>
        /// <param name="context">Context.</param>
        public override async Task ConfigureAsync(ProvisioningRecord record, IAgentContext context)
        {
            var issuer = await Did.CreateAndStoreMyDidAsync(
                context.Wallet, IssuerSeed != null ? new { seed = IssuerSeed }.ToJson() : "{}");

            record.IssuerDid = issuer.Did;
            record.IssuerVerkey = issuer.VerKey;
            record.TailsBaseUri = TailsBaseUri?.ToString();
        }
    }
}

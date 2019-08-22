using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// A configuration object for controlling the provisioning of a new agent.
    /// </summary>
    public class ProvisioningConfiguration
    {
        /// <summary>
        /// Gets or sets the defalt payment address seed
        /// </summary>
        public string AddressSeed { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner of the agent
        /// </summary>
        /// <value>
        /// The agent owner name 
        /// </value>
        public string OwnerName { get; set; }

        /// <summary>
        /// Gets or sets the imageUrl of the owner of the agent
        /// </summary>
        /// <value>
        /// The agent owner image url
        /// </value>
        public string OwnerImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the agent seed used to generate deterministic DID and Verkey. (32 characters)
        /// <remarks>Leave <c>null</c> to generate random agent did and verkey</remarks>
        /// </summary>
        /// <value>
        /// The agent seed.
        /// </value>
        public string AgentSeed { get; set; }

        /// <summary>
        /// Gets or sets the agent did.
        /// </summary>
        /// <value>
        /// The agent did.
        /// </value>
        public string AgentDid { get; set; }

        /// <summary>
        /// Gets or sets the agent verkey.
        /// </summary>
        /// <value>
        /// The agent verkey.
        /// </value>
        public string AgentVerkey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URI that this agent will receive Sovrin messages
        /// </summary>
        /// <value>
        /// The endpoint URI.
        /// </value>
        public Uri EndpointUri { get; set; }

        /// <summary>
        /// Gets or sets the wallet configuration.
        /// </summary>
        /// <value>
        /// The wallet configuration.
        /// </value>
        public WalletConfiguration WalletConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the wallet credentials.
        /// </summary>
        /// <value>
        /// The wallet credentials.
        /// </value>
        public WalletCredentials WalletCredentials { get; set; }

        /// <summary>
        /// Initial set of tags to populate the provisioning record
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the name of the pool.
        /// </summary>
        /// <value>The name of the pool.</value>
        public string PoolName { get; set; }

        /// <summary>
        /// Gets or sets the genesis filename.
        /// </summary>
        /// <value>The genesis filename.</value>
        public string GenesisFilename { get; set; }

        /// <summary>
        /// Configures the agent wallet.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="record">Record.</param>
        /// <param name="context">Context.</param>
        public virtual Task ConfigureAsync(ProvisioningRecord record, IAgentContext context) => Task.CompletedTask;

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"OwnerName={OwnerName}, " +
            $"OwnerImageUrl={OwnerImageUrl}, " +
            $"AgentSeed={(AgentSeed?.Length > 0 ? "[hidden]" : null)}, " +
            $"AgentDid={AgentDid}, " +
            $"AgentVerkey={(AgentVerkey?.Length > 0 ? "[hidden]" : null)}, " +
            $"EndpointUri={EndpointUri}, " +
            $"WalletConfiguration={WalletConfiguration}, " +
            $"WalletCredentials={WalletCredentials}";
    }
}

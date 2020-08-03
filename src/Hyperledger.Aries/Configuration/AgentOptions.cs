﻿using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using System;
using System.IO;

namespace Hyperledger.Aries.Configuration
{
    /// <summary>
    /// Agent options
    /// </summary>
    public class AgentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentOptions" /> class.
        /// </summary>
        public AgentOptions()
        {
            WalletConfiguration = new WalletConfiguration { Id = "DefaultWallet" };
            WalletCredentials = new WalletCredentials { Key = "DefaultKey" };
        }

        /// <summary>
        /// Gets or sets the wallet configuration.
        /// </summary>
        /// <value>The wallet configuration.</value>
        public WalletConfiguration WalletConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the wallet credentials.
        /// </summary>
        /// <value>The wallet credentials.</value>
        public WalletCredentials WalletCredentials
        {
            get;
            set;
        }
        /// <summary>
        /// The DID of the issuer key pair
        /// </summary>
        /// <value></value>
        public string IssuerDid { get; set; }

        /// <summary>
        /// The key of the
        /// </summary>
        /// <value></value>
        public string IssuerKeySeed { get; set; }

        /// <summary>
        /// Gets or sets the agent did
        /// </summary>
        /// <value></value>
        public string AgentDid { get; set; }

        /// <summary>
        /// Gets or sets the agent key generation seed
        /// </summary>
        /// <value></value>
        public string AgentKeySeed { get; set; }

        /// <summary>
        /// Gets or sets the agent endpoint uri
        /// </summary>
        /// <value></value>
        public string EndpointUri { get; set; }

        /// <summary>
        /// Gets or sets the agent name used in connection invitations
        /// </summary>
        /// <value></value>
        public string AgentName { get; set; }

        /// <summary>
        /// Gets or sets the agent image uri
        /// </summary>
        /// <value></value>
        public string AgentImageUri { get; set; }

        /// <summary>
        /// The verification key of the agent
        /// </summary>
        /// <value></value>
        public string AgentKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the pool.
        /// </summary>
        /// <value>The name of the pool.</value>
        public string PoolName
        {
            get;
            set;
        } = "DefaultPool";

        /// <summary>
        /// Gets or sets the genesis filename.
        /// </summary>
        /// <value>The genesis filename.</value>
        public string GenesisFilename
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the protocol version of the nodes.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public int ProtocolVersion
        {
            get;
            set;
        } = 2;

        /// <summary>
        /// Gets or sets the revocation registry URI path e.g. "/tails".
        /// This path will be appended to the EndpointUri value.
        /// This is used to optimize the ASP.NET Core middleware pipeline.
        /// Default value is "/tails". The value must start with a slash '/'.
        /// </summary>
        /// <value>
        /// The revocation registry base URI path.
        /// </value>
        public string RevocationRegistryUriPath { get; set; } = "/tails";

        /// <summary>
        /// Gets or sets the revocation registry directory where
        /// revocation tails files will be stored. The default path 
        /// is ~/.indy_client/tails
        /// </summary>
        /// <value>
        /// The revocation registry directory.
        /// </value>
        public string RevocationRegistryDirectory { get; set; } = EnvironmentUtils.GetTailsPath();

        /// <summary>
        /// Gets or sets the backup directory where physical backups will be stored.
        /// This property is only used when running a mediator service.
        /// </summary>
        /// <value>
        /// The backup directory.
        /// </value>
        public string BackupDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "AriesBackups");

        /// <summary>
        /// Automatically respond to credential offers with a credential request. Default: false
        /// </summary>
        /// <value>The name of the pool.</value>
        public bool AutoRespondCredentialOffer
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Automatically respond to credential request with corresponding credentials. Default: false
        /// </summary>
        /// <value>The name of the pool.</value>
        public bool AutoRespondCredentialRequest
        {
            get;
            set;
        } = false;
    }
}

using System;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Routing
{
    public class InboxRecord : RecordBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InboxRecord"/> class.
        /// </summary>
        public InboxRecord() => Id = Guid.NewGuid().ToString();
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public override string TypeName => "Hyperledger.Aries.Routing.InboxRecord";

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
    }
}
﻿using Newtonsoft.Json;

namespace Hyperledger.Aries.Storage
{
    public partial class WalletConfiguration
    {
        /// <summary>
        /// Wallet storage configuration.
        /// </summary>
        public class WalletStorageConfiguration
        {
            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            [JsonProperty("path")]
            public string Path { get; set; }

            /// <inheritdoc />
            public override string ToString() =>
                $"{GetType().Name}: " +
                $"Path={Path}";
        }
    }
}

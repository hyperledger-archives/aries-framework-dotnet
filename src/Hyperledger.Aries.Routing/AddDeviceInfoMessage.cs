using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Add Device Info Message
    /// </summary>
    /// <seealso cref="Hyperledger.Aries.Agents.AgentMessage" />
    public class AddDeviceInfoMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddDeviceInfoMessage"/> class.
        /// </summary>
        public AddDeviceInfoMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.AddDeviceInfoMessage;
        }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public string DeviceId { get; set; }
        /// <summary>
        /// Gets or sets the device vendor.
        /// </summary>
        /// <value>
        /// The device vendor.
        /// </value>
        public string DeviceVendor { get; set; }

        /// <summary>
        /// Gets or sets the device metadata.
        /// </summary>
        /// <value>
        /// The device metadata.
        /// </value>
        public Dictionary<string, string> DeviceMetadata { get; set; }
    }
}

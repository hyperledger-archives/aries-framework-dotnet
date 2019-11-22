using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperledger.Aries.Routing
{
    public class AddDeviceInfoMessage : AgentMessage
    {
        public AddDeviceInfoMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.AddDeviceInfoMessage;
        }

        public string DeviceId { get; set; }
        public string DeviceVendor { get; set; }

        public Dictionary<string, string> DeviceMetadata { get; set; }
    }
}

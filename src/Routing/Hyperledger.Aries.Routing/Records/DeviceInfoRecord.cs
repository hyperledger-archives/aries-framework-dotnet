using Hyperledger.Aries.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperledger.Aries.Routing
{
    public class DeviceInfoRecord : RecordBase
    {
        public DeviceInfoRecord()
        {
            Id = Guid.NewGuid().ToString();
        }

        public override string TypeName => "Hyperledger.Aries.Routing.DeviceInfoRecord";

        public string DeviceId { get; set; }

        public string DeviceVendor { get; set; }

        [JsonIgnore]
        public string InboxId { get => Get(); set => Set(value); }
    }
}

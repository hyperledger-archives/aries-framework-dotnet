using Hyperledger.Aries.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperledger.Aries.Routing
{
    public class RouteRecord : RecordBase
    {
        public override string TypeName => "Hyperledger.Aries.Routing.RouteRecord";

        [JsonIgnore]
        public string InboxId { get => Get(); set => Set(value); }
    }
}

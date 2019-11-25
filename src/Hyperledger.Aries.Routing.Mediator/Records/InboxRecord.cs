using System;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Routing
{
    public class InboxRecord : RecordBase
    {
        public InboxRecord() => Id = Guid.NewGuid().ToString();
        public override string TypeName => "Hyperledger.Aries.Routing.InboxRecord";

        public WalletConfiguration WalletConfiguration { get; set; }

        public WalletCredentials WalletCredentials { get; set; }
    }
}
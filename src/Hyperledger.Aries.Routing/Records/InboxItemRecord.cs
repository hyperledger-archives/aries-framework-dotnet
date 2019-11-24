using Hyperledger.Aries.Storage;
using System;

namespace Hyperledger.Aries.Routing
{
    public class InboxItemRecord : RecordBase
    {
        public InboxItemRecord() => Id = Guid.NewGuid().ToString();

        /// <summary>
        /// String representation of binary data in UTF-8 byte mark
        /// </summary>
        /// <value></value>
        public string ItemData { get; set; }

        public override string TypeName => "Hyperledger.Aries.Routing.InboxItemRecord";
    }
}
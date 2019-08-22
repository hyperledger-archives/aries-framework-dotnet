using System;
using System.Collections.Generic;
using System.Linq;
using AgentFramework.Core.Models.Payments;
using Newtonsoft.Json;
// ReSharper disable VirtualMemberCallInConstructor

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Payment address record
    /// </summary>
    /// <seealso cref="AgentFramework.Core.Models.Records.RecordBase" />
    public class PaymentAddressRecord : RecordBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAddressRecord"/> class.
        /// </summary>
        public PaymentAddressRecord()
        {
            Id = Guid.NewGuid().ToString();
            Sources = new List<IndyPaymentInputSource>();
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public override string TypeName => "AF.PaymentAddress";

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [JsonIgnore]
        public string Address
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonIgnore]
        public string Method
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        [JsonIgnore]
        public ulong Balance =>
            Sources.Any()
            ? Sources.Select(x => x.Amount).Aggregate((x, y) => x + y)
            : 0;

        /// <summary>
        /// Gets or sets the sources synced at.
        /// </summary>
        /// <value>
        /// The sources synced at.
        /// </value>
        public DateTime SourcesSyncedAt { get; set; }

        /// <summary>
        /// Gets or sets the payment sources as unspent transaction objects (UTXO).
        /// </summary>
        /// <value>
        /// The sources.
        /// </value>
        public IList<IndyPaymentInputSource> Sources { get; set; }
    }
}

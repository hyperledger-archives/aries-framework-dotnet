namespace Hyperledger.Aries.Ledger.Models
{
    public class ParseRegistryResponseResult
    {
        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>Gets the object json.</summary>
        /// <value>The object json.</value>
        public string ObjectJson { get; }

        /// <summary>Gets the timestamp.</summary>
        /// <value>The timestamp.</value>
        public ulong Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseRegistryResponseResult" /> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="objectJson">Object json.</param>
        /// <param name="timestamp">Timestamp.</param>
        internal ParseRegistryResponseResult(string id, string objectJson, ulong timestamp)
        {
            this.Id = id;
            this.ObjectJson = objectJson;
            this.Timestamp = timestamp;
        }
    }
}

namespace Hyperledger.Aries.Routing
{
    public class InboxItemEvent
    {
        /// <summary>
        /// Gets or sets the inbox identifier.
        /// </summary>
        /// <value>
        /// The inbox identifier.
        /// </value>
        public string InboxId { get; set; }
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemId { get; set; }
    }
}

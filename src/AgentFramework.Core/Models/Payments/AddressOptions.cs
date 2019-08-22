namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment account configuration.
    /// </summary>
    public class AddressOptions
    {
        /// <summary>
        /// Account identifier
        /// </summary>
        public string Seed { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; set; }
    }
}
namespace AgentFramework.Core.Models
{
    /// <summary>
    /// An object for containing agent ownership information.
    /// </summary>
    public class AgentOwner
    {
        /// <summary>
        /// Gets or sets the name of the owner of the agent.
        /// </summary>
        /// <value>
        /// The agent owners name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the image url of the owner of the agent.
        /// </summary>
        /// <value>
        /// The agent owners image url.
        /// </value>
        public string ImageUrl { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"ImageUrl={ImageUrl}";
    }
}

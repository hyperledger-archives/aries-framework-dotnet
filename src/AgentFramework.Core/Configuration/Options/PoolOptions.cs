namespace AgentFramework.Core.Configuration.Options
{
    /// <summary>
    /// Pool options.
    /// </summary>
    public class PoolOptions
    {
        /// <summary>
        /// Gets or sets the name of the pool.
        /// </summary>
        /// <value>The name of the pool.</value>
        public string PoolName
        {
            get;
            set;
        } = "DefaultPool";

        /// <summary>
        /// Gets or sets the genesis filename.
        /// </summary>
        /// <value>The genesis filename.</value>
        public string GenesisFilename
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the protocol version of the nodes.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public int ProtocolVersion
        {
            get;
            set;
        } = 2;

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"PoolName={PoolName}, " +
            $"GenesisFilename={GenesisFilename}, " +
            $"ProtocolVersion={ProtocolVersion}";
    }
}
